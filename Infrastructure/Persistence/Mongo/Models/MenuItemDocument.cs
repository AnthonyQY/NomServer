using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NomServer.Infrastructure.Persistence.Mongo.Models;

public class MenuItemDocument
{

    [BsonId]
    [BsonRequired]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonRequired]
    [BsonRepresentation(BsonType.String)]
    [BsonElement("Name")]
    public string Name { get; set; }
    
    [BsonRequired]
    [BsonRepresentation(BsonType.Int32)]
    [BsonElement("Quantity")]
    public int Quantity { get; set; }
    
    [BsonRequired]
    [BsonRepresentation(BsonType.Boolean)]
    [BsonElement("IsAvailable")]
    public bool IsAvailable { get; set; }
}