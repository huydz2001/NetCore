
using DnsClient.Protocol;
using Microsoft.Extensions.Options;
using MobileBackend.Common.Constants.Table;
using MobileBackend.Models;
using MobileBackend.Models.Home;
using MobileBackend.Models.User;
using MongoDB.Driver;

namespace MobileBackend.Common.Func;
public class FuncCommons
{
    private readonly IMongoCollection<Home> _homeCollection;
    private readonly IMongoCollection<User> _userCollection;
    private readonly MongoClient _mongoclient;

    public FuncCommons(IOptions<MongoDbSetting> mogoDbSetting)
    {
        MongoClient mongoClient = new MongoClient(mogoDbSetting.Value.ConnectionURI);
        IMongoDatabase database = mongoClient.GetDatabase(mogoDbSetting.Value.DatabaseName);
        _homeCollection = database.GetCollection<Home>(HomeTableConstants.HomeTable);
        _userCollection = database.GetCollection<User>(UserTableConstants.UserTable);
        _mongoclient = mongoClient;
    }


    public async Task<bool> UpdateRoleMember(string userId, int role)
    {
        try
        {

            var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);
            var update = Builders<User>.Update
                            .Set(x => x.Role, role)
                            .Set(x => x.UpdateDate, DateTime.Now);
            var result = await _userCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> GetUserRole(string userId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);
        var user = await _userCollection.Find(filter).FirstOrDefaultAsync();

        if (user != null)
        {
            return user.Role;
        }

        throw new Exception("User not found");
    }

}