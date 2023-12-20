using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MobileBackend.Models.Home;


public class Member
{
    [BsonElement("userId")]
    public required string UserId { get; set; }

    [BsonElement("roleHome")]
    public required int RoleHome { get; set; } = 0;

    [BsonElement("status")]
    public required int Status { get; set; } = 1;     // 0: join vao  , 1: admin them, 2: da join thanh cong
}

public class Home
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
    public required List<Member> Members { get; set; }

    [BsonElement("createBy")]
    public required string CreatedBy { get; set; }

    [BsonElement("createDate")]
    public DateTime CreateDate { get; set; }

    [BsonElement("isPublic")]
    public int IsPublic { get; set; } = 0;

    [BsonElement("isDel")]
    public int IsDel { get; set; } = 0;

    [BsonElement("updateBy")]
    public string? UpdateBy { get; set; }

    [BsonElement("updateDate")]
    public DateTime? UpdateDate { get; set; }

    [BsonElement("delDate")]
    public DateTime? DelDate { get; set; }
}