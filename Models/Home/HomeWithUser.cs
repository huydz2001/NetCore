using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MobileBackend.Models.Home;


public class HomeWithUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string? Id { get; set; }

    [BsonElement("homeId")]
    public string? HomeId { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("image")]
    public string? Image { get; set; }

    [BsonElement("slogan")]
    public string? Slogan { get; set; }

    [BsonElement("members")]
    public List<Member>? Members { get; set; }

    [BsonElement("createBy")]
    public string? CreatedBy { get; set; }

    [BsonElement("createDate")]
    public DateTime CreateDate { get; set; }

    [BsonElement("isPublic")]
    public int IsPublic { get; set; } = 0;

    [BsonElement("isDel")]
    public int IsDel { get; set; } = 0;

    [BsonElement("delDate")]
    public DateTime? DelDate { get; set; }

    [BsonElement("memberDetails")]
    public List<User.User>? MemberDetails { get; set;}
}