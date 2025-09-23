using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NomServer.Core.Entities;

namespace NomServer.Infrastructure.Persistence.Mongo.Models;

public class OrderDocument
{

    [BsonId]
    [BsonRequired]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonRequired]
    [BsonRepresentation(BsonType.String)]
    [BsonElement("Username")]
    public string Username { get; set; }
    
    [BsonRequired]
    [BsonRepresentation(BsonType.DateTime)]
    [BsonElement("DatePlaced")]
    public DateTime DatePlaced { get; set; }
    
    [BsonRequired]
    [BsonRepresentation(BsonType.Int32)]
    [BsonElement("Status")]
    public int Status { get; set; }
    
    [BsonRequired]
    [BsonElement("MenuItems")]
    public List<MenuItemDocument> MenuItems { get; set; }
}