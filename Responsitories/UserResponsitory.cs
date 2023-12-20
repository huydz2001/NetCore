using MongoDB.Driver;
using MobileBackend.IResponsitory;
using MobileBackend.Models.User;
using Microsoft.Extensions.Options;
using MobileBackend.Models;
using MongoDB.Bson;
using MobileBackend.Models.Task;
using MobileBackend.Models.Home;
using MobileBackend.Common.Constants;
using MobileBackend.Common.Constants.Table;
using MobileBackend.Common.Func;

namespace MobileBackend.Responsitories;
public class UserResponsitory : IUserRepository
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Tasks> _taskCollection;
    private readonly IMongoCollection<Home> _homeCollection;
    private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;
    private readonly FuncCommons funcCommons;

    public UserResponsitory(IOptions<MongoDbSetting> mogoDbSetting)
    {
        MongoClient mongoClient = new MongoClient(mogoDbSetting.Value.ConnectionURI);
        IMongoDatabase database = mongoClient.GetDatabase(mogoDbSetting.Value.DatabaseName);
        _userCollection = database.GetCollection<User>(UserTableConstants.UserTable);
        _taskCollection = database.GetCollection<Tasks>(TaskTableConstants.TaskTable);
        _homeCollection = database.GetCollection<Home>(HomeTableConstants.HomeTable);
        _refreshTokenCollection = database.GetCollection<RefreshToken>(UserTableConstants.RefreshTokenTable);
        funcCommons = new FuncCommons(mogoDbSetting);

    }

    public async Task<List<User>> GetAllUsers()
    {

        var filter = Builders<User>.Filter.Empty;
        var users = await _userCollection.Find(filter).ToListAsync();

        return users;

    }
    public async Task<User> GetUserById(string userId)
    {

        var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);
        var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
        return user;

    }
    public async Task<bool> Register(User user)
    {

        var filter = Builders<User>.Filter.Eq(x => x.Contact.Phone, user.Contact.Phone);
        var result = await _userCollection.Find(filter).FirstOrDefaultAsync();

        if (result != null)
        {
            throw new Exception("Phone number is already registered");
        }
        else
        {
            await _userCollection.InsertOneAsync(user);
            return true;
        }

    }
    public async Task<bool> UpdateUser(string userId, UserInfor user)
    {

        var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);
        var update = Builders<User>.Update
        .Set(u => u.Name, user.Name)
        .Set(u => u.Age, user.Age)
        .Set(u => u.Dob, user.Dob)
        .Set(u => u.Contact, user.Contact)
        .Set(u => u.Role, user.Role)
        .Set(u => u.UpdateDate, DateTime.Now);

        var result = await _userCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;

    }
    public async Task<User> Login(UserLogin userLogin)
    {

        var filter = Builders<User>.Filter.Eq(u => u.Contact.Phone, userLogin.Phone) &
                     Builders<User>.Filter.Eq(u => u.Password, userLogin.Password);

        var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
        return user;

    }

    public async Task<User> GetUserByPhone(string phoneNumber)
    {

        var filter = Builders<User>.Filter.Eq(x => x.Contact.Phone, phoneNumber);

        var user = await _userCollection.Find(filter).FirstOrDefaultAsync();

        return user;

    }

    public async Task<List<Tasks>> GetTaskOfUser(string userId, DateOnly date)
    {

        var filter = Builders<Tasks>.Filter.And(
            Builders<Tasks>.Filter.Eq(x => x.Owner, userId),
            Builders<Tasks>.Filter.Eq(x => x.StartDate, date)
        );

        List<Tasks> tasks = await _taskCollection.Find(filter).ToListAsync();
        return tasks;

    }

    public async Task<bool> AcceptHome(string userId, string homeId)
    {


        var filter = Builders<Home>.Filter.And(
            Builders<Home>.Filter.ElemMatch(x => x.Members, m => m.UserId == userId),
            Builders<Home>.Filter.Eq(x => x.HomeId, homeId)
        );
        var update = Builders<Home>.Update.Set("members.$.status", RoleUserConstants.StatusAccept);
        var result = await _homeCollection.UpdateOneAsync(filter, update);

        await funcCommons.UpdateRoleMember(userId, RoleUserConstants.JoinedHome);

        var filter2 = Builders<Home>.Filter.Ne(x => x.HomeId, homeId);
        var update2 = Builders<Home>.Update.PullFilter(x => x.Members, m => m.UserId == userId);
        var result2 = await _homeCollection.UpdateManyAsync(filter2, update2);


        if (result.ModifiedCount > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<List<Home>> GetHomeByUserId(string userId)
    {

        var filter = Builders<Home>.Filter.ElemMatch(x => x.Members, m => m.UserId == userId);
        var homes = await _homeCollection.Find(filter).ToListAsync();
        return homes;

    }

    public async Task<bool> OutHome(string userId, string homeId)
    {

        var filter = Builders<Home>.Filter.Eq(x => x.HomeId, homeId);
        var update = Builders<Home>.Update.PullFilter(x => x.Members, m => m.UserId == userId);
        var result = await _homeCollection.UpdateManyAsync(filter, update);
        await funcCommons.UpdateRoleMember(userId, RoleUserConstants.NotJoinHome);

        return result.ModifiedCount > 0;

    }

    public async Task<bool> AddRefreshToken(RefreshToken refreshToken)
    {
        await _refreshTokenCollection.InsertOneAsync(refreshToken);
        return true;
    }

    public async Task<RefreshToken> GetRefreshToken(string refreshToken)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, refreshToken);
        return await _refreshTokenCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool?> UpdateImageUser(string imagePath, string userId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);

        var update = Builders<User>.Update.Set(x => x.Image, imagePath);

        var result = await _userCollection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;

    }
}