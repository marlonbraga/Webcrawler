using HtmlAgilityPack;
using PuppeteerSharp;

namespace Webcrawler.Domain;

public class WebCrawler(ILogRepository logRepository)
{
    private const string BaseUrl = "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
    private const string ChromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
    private const int MaxTasksSimultaneas = 3;
    public ILogRepository LogRepository { get; set; } = logRepository;
    private readonly SemaphoreSlim _semaphore = new(MaxTasksSimultaneas);

    public async Task RunAsync()
    {
        var startTime = DateTime.UtcNow;
        var dataList = new List<ProxyData>();
        int totalPages = await GetTotalPagesAsync();

        var tasks = new List<Task>();

        for (int currentPage = 1; currentPage <= totalPages; currentPage++)
        {
            await _semaphore.WaitAsync();

            tasks.Add(ProcessPageAsync(currentPage, dataList));
        }

        await Task.WhenAll(tasks);

        await LogRepository.PersistDataAsync(dataList, startTime, totalPages);
    }

    private async Task ProcessPageAsync(int currentPage, List<ProxyData> dataList)
    {
        try
        {
            string url = $"{BaseUrl}?page={currentPage}";
            string html;

            LaunchOptions options = new()
            {
                Headless = true,
                ExecutablePath = ChromePath
            };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var pageInstance = await browser.NewPageAsync())
            {
                await pageInstance.GoToAsync(url);
                html = await pageInstance.GetContentAsync();
                HtmlDocument doc = new();
                doc.LoadHtml(html);

                var rows = doc.DocumentNode.SelectNodes("//tr");

                if (rows != null)
                {
                    foreach (var row in rows.Skip(1))
                    {
                        var cols = row.SelectNodes("td");
                        if (cols == null)
                            continue;
                        lock (dataList)
                        {
                            dataList.Add(new ProxyData
                            {
                                IpAddress = cols[1].InnerText.Trim(),
                                Port = cols[2].InnerText.Trim(),
                                Country = cols[3].InnerText.Trim(),
                                Protocol = cols[6].InnerText.Trim()
                            });
                        }
                    }
                }

                await CaptureScreenshotAsync(pageInstance, currentPage, html);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<int> GetTotalPagesAsync()
    {
        LaunchOptions options = new()
        {
            Headless = true,
            ExecutablePath = ChromePath
        };

        using var browser = await Puppeteer.LaunchAsync(options);
        using var pageInstance = await browser.NewPageAsync();
        await pageInstance.GoToAsync($"{BaseUrl}?page=1");

        string html = await pageInstance.GetContentAsync();
        HtmlDocument doc = new();
        doc.LoadHtml(html);

        var paginationNodes = doc.DocumentNode.SelectNodes("//ul[contains(@class, 'pagination')]/li/a");
        if (paginationNodes != null)
        {
            return paginationNodes
                .Select(node => int.TryParse(node.InnerText.Trim(), out int pageNumber) ? pageNumber : 0)
                .Max();
        }

        return 1;
    }

    private async Task CaptureScreenshotAsync(IPage pageInstance, int page, string html)
    {
        await pageInstance.SetContentAsync(html);
        await pageInstance.ScreenshotAsync($"../../../../CapturedData/page_{page}.png", new ScreenshotOptions
        {
            FullPage = true
        });
    }
}
