using MobileBackend.Models;
using MobileBackend.Models.Home;
using MobileBackend.Models.Task;
using MobileBackend.Models.User;

namespace MobileBackend.IService;
public interface IUserService
{
    public Task<List<User>> GetAllUsers();

    public Task<User> GetUserById(string userId);

    public Task<bool> Register(UserRegister user);

    public Task<bool> UpdateUser(string userId, UserInfor user);

    public Task<User> Login(UserLogin userLogin);

    public Task<User> GetUserByPhone(string phone);

    Task<List<Tasks>> GetTaskOfUser(string userId, DateOnly date);

    Task<List<Home>> GetHomeByUserId(string userId);

    Task<bool> AcceptHome(string userId, string homeId);

    Task<bool> OutHome(string userId, string homeId);

    Task<bool> AddRefreshToken(RefreshToken refreshToken);
    
    Task<RefreshToken> GetRefreshToken (string refreshToken);

    Task<bool?> UpdateImageUser(string imagePath, string id);

}