using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MobileBackend.Models.User;

public class Contact
{
    [BsonElement("email")]
    public string? Email { get; set; } = null;

    [BsonElement("phone")]
    public string? Phone { get; set; }

    public Contact(string? email, string? phone)
    {
        Email = email;
        Phone = phone;
    }
}
