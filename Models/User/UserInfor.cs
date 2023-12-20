using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MobileBackend.Models.User;


public class UserInfor
{

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("age")]
    public int? Age { get; set; }

    [BsonElement("dob")]
    public DateOnly? Dob {get; set;} = null;

    [BsonElement("contact")]
    public required Contact Contact { get; set; }

    [BsonElement("role")]
    public int? Role { get; set; }

    [BsonElement("updateDate")]
    public DateTime? UpdateDate { get; set; } = DateTime.Now;

}