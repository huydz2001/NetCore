using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MobileBackend.Models.User;

public class UserLogin
{

    [BsonElement("phone")]
    public required string Phone {get; set;}

    [BsonElement("password")]
    public required string Password { get; set; }


}