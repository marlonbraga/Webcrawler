using System.Text.Json;
using Webcrawler.Domain;

namespace Webcrawler.Data
{
    public class LogRepository : ILogRepository
    {        
        public async Task PersistDataAsync(List<ProxyData> dataList, DateTime startTime, int currentPage)
        {
            await SaveToJson(dataList);
            await LogToDatabase(dataList, startTime, currentPage);
        }

        private async Task SaveToJson(List<ProxyData> dataList)
        {
            string jsonFilePath = $"../../../../CapturedData/output_{DateTime.UtcNow.Ticks}.json";
            await File.WriteAllTextAsync(jsonFilePath, JsonSerializer.Serialize(dataList));
        }

        private async Task LogToDatabase(List<ProxyData> dataList, DateTime startTime, int currentPage)
        {
            var context = new CrawlerContext();

            var log = new CrawlerLog
            {
                Id = new Guid(),
                StartTime = startTime,
                EndTime = DateTime.UtcNow,
                TotalPages = currentPage - 1,
                TotalRows = dataList.Count,
                DataList = dataList
            };

            await context.Logs.InsertOneAsync(log);
        }
    }
}
