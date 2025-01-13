using HtmlAgilityPack;
using PuppeteerSharp;

namespace Webcrawler.Domain;

public class WebCrawler(ILogRepository logRepository)
{
    private const string BaseUrl = "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
    private const string ChromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
    public ILogRepository LogRepository { get; set; } = logRepository;
    private readonly SemaphoreSlim _semaphore = new(3);

    public async Task RunAsync()
    {
        var startTime = DateTime.UtcNow;
        var dataList = new List<ProxyData>();
        int currentPage = 1;
        bool hasNextPage = true;

        LaunchOptions options = new()
        {
            Headless = true,
            ExecutablePath = ChromePath
        };
        using (var browser = await Puppeteer.LaunchAsync(options))
            while (hasNextPage)
            {
                await _semaphore.WaitAsync();

                try
                {
                    string url = $"{BaseUrl}?page={currentPage}";
                    string html;
                    HtmlNodeCollection rows;

                    using (var pageInstance = await browser.NewPageAsync())
                    {
                        await pageInstance.GoToAsync(url);
                        html = await pageInstance.GetContentAsync();
                        HtmlDocument doc = new();
                        doc.LoadHtml(html);
                        rows = doc.DocumentNode.SelectNodes("//tr");

                        if (rows.Count == 1)
                        {
                            hasNextPage = false;
                            break;
                        }
                        await CaptureScreenshotAsync(pageInstance, currentPage, html);
                    }

                    foreach (var row in rows.Skip(1))
                    {
                        var cols = row.SelectNodes("td");
                        if (cols == null)
                            continue;
                        dataList.Add(new ProxyData
                        {
                            IpAddress = cols[1].InnerText.Trim(),
                            Port = cols[2].InnerText.Trim(),
                            Country = cols[3].InnerText.Trim(),
                            Protocol = cols[6].InnerText.Trim()
                        });
                    }

                    currentPage++;
                }
                finally
                {
                    _semaphore.Release();
                }
            }

        await LogRepository.PersistDataAsync(dataList, startTime, currentPage);
    }

    private async Task CaptureScreenshotAsync(IPage pageInstance, int page, string html)
    {
        await pageInstance.SetContentAsync(html);
        await pageInstance.ScreenshotAsync($"page_{page}.png", new ScreenshotOptions
        {
            FullPage = true
        });
    }
}
