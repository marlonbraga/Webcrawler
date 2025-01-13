namespace Webcrawler.Domain;

public interface ILogRepository
{
    Task PersistDataAsync(List<ProxyData> dataList, DateTime startTime, int currentPage);
}
