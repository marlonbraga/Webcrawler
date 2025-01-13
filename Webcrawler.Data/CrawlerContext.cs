using MongoDB.Driver;

namespace Webcrawler.Data;

public class CrawlerContext
{
    private readonly IMongoDatabase _database;

    public CrawlerContext()
    {
        var connectionString = "mongodb://admin:secret@localhost:27017/WebCrawlerDb?authSource=admin&authMechanism=SCRAM-SHA-256";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("WebCrawlerDb");
    }

    public IMongoCollection<CrawlerLog> Logs => _database.GetCollection<CrawlerLog>("CrawlerLogs");
}
