using Webcrawler.Data;
using Webcrawler.Domain;

Console.WriteLine("Initialize webcrawling");

ILogRepository logRepository = new LogRepository();
WebCrawler webCrawler = new(logRepository);

await webCrawler.RunAsync();

Console.WriteLine("Webcrawling finished");