using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MobileBackend.Common.Constants;
using MobileBackend.Common.Constants.Table;
using MobileBackend.IResponsitory;
using MobileBackend.IService;
using MobileBackend.Models;
using MobileBackend.Models.Home;
using MobileBackend.Models.User;
using MongoDB.Bson;
using MongoDB.Driver;
using MobileBackend.Common.Func;

namespace MobileBackend.Responsitories;

public class HomeResponsitory : IHomeResponsitory
{
    private readonly IMongoCollection<Home> _homeCollection;
    private readonly IMongoCollection<User> _userCollection;
    private readonly MongoClient _mongoclient;
    private readonly FuncCommons funcCommons;


    public HomeResponsitory(IOptions<MongoDbSetting> mogoDbSetting, IUserService userService)
    {
        MongoClient mongoClient = new MongoClient(mogoDbSetting.Value.ConnectionURI);
        IMongoDatabase database = mongoClient.GetDatabase(mogoDbSetting.Value.DatabaseName);
        _homeCollection = database.GetCollection<Home>(HomeTableConstants.HomeTable);
        _userCollection = database.GetCollection<User>(UserTableConstants.UserTable);
        _mongoclient = mongoClient;
        funcCommons = new FuncCommons(mogoDbSetting);
    }

    public async Task<HomeWithUser> GetById(string homeId)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("homeId", homeId)),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", UserTableConstants.UserTable },
                { "localField", HomeTableConstants.HomeLocalField },
                { "foreignField", HomeTableConstants.HomeForeignField },
                { "as", HomeTableConstants.HomeJoinResult }
            })
        };

        var cursor = await _homeCollection.AggregateAsync<HomeWithUser>(pipeline);
        return await cursor.FirstOrDefaultAsync();


    }


    public async Task<bool> Create(Home home)
    {

            await _homeCollection.InsertOneAsync(home);
            bool success = await funcCommons.UpdateRoleMember(home.CreatedBy, RoleUserConstants.CreateHome);
            if (success)
            {
                return true;
            }
            return false;

    }


    public async Task<List<Home>> GetAll()
    {
        var filter = Builders<Home>.Filter.Empty;
        List<Home> homes = await _homeCollection.Find(filter).ToListAsync();
        return homes;
    }

    public async Task<bool> AddMember(string homeId, Member member)
    {
        var existingFilter = Builders<Home>.Filter.And(
            Builders<Home>.Filter.Eq(x => x.HomeId, homeId),
            Builders<Home>.Filter.ElemMatch(x => x.Members, m => m.UserId == member.UserId)
        );

        var existingMember = await _homeCollection.Find(existingFilter).FirstOrDefaultAsync();

        var role = funcCommons.GetUserRole(member.UserId).Result;

        if (role >= RoleUserConstants.JoinedHome)
        {
            throw new Exception("User joined a home other");
        }

        else if (existingMember != null)
        {
            throw new Exception("Invited this user");
        }


        var filter = Builders<Home>.Filter.Eq(x => x.HomeId, homeId);
        var update = Builders<Home>.Update.Push(x => x.Members, member);
        var result = await _homeCollection.UpdateOneAsync(filter, update);
        await funcCommons.UpdateRoleMember(member.UserId, RoleUserConstants.InvitedHome);

        return result.ModifiedCount > 0;
    }


    public async Task<bool> DeleteMember(string homeId, string userId)
    {
        var filter = Builders<Home>.Filter.And(
                    Builders<Home>.Filter.Eq(x => x.HomeId, homeId),
                    Builders<Home>.Filter.ElemMatch(x => x.Members, m => m.UserId == userId)
        );

        var update = Builders<Home>.Update.PullFilter(x => x.Members, m => m.UserId == userId);
        var result = await _homeCollection.UpdateOneAsync(filter, update);
        var result2 = await funcCommons.UpdateRoleMember(userId, RoleUserConstants.NotJoinHome);

        if (result.ModifiedCount == 0 || !result2)
        {
            throw new Exception("Don't have any member with id: " + userId);
        }
        return true;


    }

    public async Task<bool> UpdateHome(string homeId, HomeInfor home)
    {

        var filter = Builders<Home>.Filter.Eq(X => X.HomeId, homeId);
        var update = Builders<Home>.Update
                        .Set(x => x.Name, home.Name)
                        .Set(x => x.IsPublic, home.IsPublic)
                        .Set(x => x.Slogan, home.Slogan)
                        .Set(x => x.Image, home.Image)
                        .Set(x => x.UpdateBy, home.Admin)
                        .Set(x => x.UpdateDate, DateTime.Now);

        var result = await _homeCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteHome(string homeId)
    {

        var pipeline = new BsonDocument[]
        {
                BsonDocument.Parse("{ $unwind: '$members' }"),
                BsonDocument.Parse("{ $match: {homeId:'" + homeId + "'  } }"),
                BsonDocument.Parse("{ $project: {userId: '$members.userId' } }")
        };

        var userIds = _homeCollection
                        .Aggregate<BsonDocument>(pipeline)
                        .ToList()
                        .Select(x => x.GetValue("userId").AsString)
                        .ToList();

        var filter = Builders<User>.Filter.In(x => x.UserId, userIds);
        var update = Builders<User>.Update.Set(x => x.Role, RoleUserConstants.NotJoinHome);

        var updateResult = await _userCollection.UpdateManyAsync(filter, update);
        if (updateResult.ModifiedCount > 0)
        {
            return true;
        }
        return false;

    }

}