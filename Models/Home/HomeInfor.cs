using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MobileBackend.Models.Home;

public class HomeInfor
{
    [BsonElement("admin")]
    public string? Admin { get; set;}

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("image")]
    public string? Image { get; set; }

    [BsonElement("slogan")]
    public string? Slogan { get; set; }

    [BsonElement("isPublic")]
    public int IsPublic { get; set; } = 0;

    [BsonElement("isDel")]
    public int IsDel { get; set; } = 0;


}