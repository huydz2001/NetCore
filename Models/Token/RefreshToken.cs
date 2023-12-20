
using MongoDB.Bson;
using MobileBackend.Models.User;
using MongoDB.Bson.Serialization.Attributes;

namespace MobileBackend.Models;

    public class RefreshToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; } = null!;

    [BsonElement("token")]
    public string Token { get; set; } = null!;
    
    [BsonElement("jwtId")]
    public string JwtId { get; set; } = null!;

    [BsonElement("usUsed")]
    public bool IsUsed { get; set; }

    [BsonElement("isRevoked")]
    public bool IsRevoked { get; set; }

    [BsonElement("issuedAt")]
    public DateTime IssuedAt { get; set; }

    [BsonElement("expiredAt")]
    public DateTime ExpiredAt { get; set; }
}
