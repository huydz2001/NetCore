using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MobileBackend.Models.User;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("age")]
    public int? Age { get; set; }

    [BsonElement("dob")]
    public DateOnly? Dob {get; set;} = null;

    [BsonElement("contact")]
    public Contact Contact { get; set; }

    [JsonIgnore] 
    [BsonElement("password")]
    public string Password { get; set; }

    [BsonElement("role")]
    public int Role { get; set; } = 0;    // 0: chua join home,   1: da join home, 2: tao home

    [BsonElement("image")]
    public string? Image { get; set; } = null;

    [BsonElement("tasks")]
    public List<string>? Tasks { get; set; } = null;

    [BsonElement("createDate")]
    public DateTime CreateDate { get; set; } = DateTime.Now;

    [BsonElement("updateDate")]
    public DateTime? UpdateDate { get; set; } = null;

    public User(string userId,string name, int? age,DateOnly ? dob, Contact contact, string password, int role, string? image, List<string>? tasks, DateTime createDate, DateTime? updateDate)
    {
        UserId = userId;
        Name = name;
        Age = age;
        Dob = dob;
        Contact = contact;
        Password = password;
        Role = role;
        Image = image;
        Tasks = tasks;
        CreateDate = createDate;
        UpdateDate = updateDate;
    }

}