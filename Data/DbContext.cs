using MongoDB.Driver;

namespace MobileBackend.Data;
public class MyDbContext
{
    private readonly IMongoDatabase _database;

    public MyDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionURI");
        var mongoClient = new MongoClient(connectionString);
        _database = mongoClient.GetDatabase("DatabaseName");
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}