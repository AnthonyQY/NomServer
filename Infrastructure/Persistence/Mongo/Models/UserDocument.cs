using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NomServer.Infrastructure.Persistence.Mongo.Models;

public class UserDocument
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
    [BsonElement("Roles")]
    public List<string> Roles { get; set; }
    
    [BsonRequired]
    [BsonRepresentation(BsonType.String)]
    [BsonElement("RecoveryCodeHash")]
    public string RecoveryCodeHash { get; set; }
    
    [BsonRequired]
    [BsonRepresentation(BsonType.String)]
    [BsonElement("RecoveryCodeSalt")]
    public string RecoveryCodeSalt { get; set; }
    
    [BsonRequired]
    [BsonRepresentation(BsonType.Boolean)]
    [BsonElement("IsActive")]
    public bool IsActive { get; set; }
}