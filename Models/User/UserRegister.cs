using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MobileBackend.Models.User;
public class UserRegister
{
    [BsonElement("userId")]
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("phone")]
    public required string Phone { get; set; }

    [BsonElement("password")]
    public required string Password { get; set; }
}