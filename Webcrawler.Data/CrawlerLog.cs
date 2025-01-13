using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Webcrawler.Domain;

namespace Webcrawler.Data;

public class CrawlerLog
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int? TotalPages { get; set; }
    public int? TotalRows { get; set; }
    public required List<ProxyData> DataList { get; set; }
}
