using MobileBackend.Models;
using MobileBackend.Models.Home;
using MobileBackend.Models.Task;
using MobileBackend.Models.User;

namespace MobileBackend.IResponsitory;
public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    
    Task<User> GetUserById(string userId);

    Task<bool> Register(User user);

    Task<bool> UpdateUser(string userId, UserInfor user);

    Task<User> Login(UserLogin userLogin);

    Task<User> GetUserByPhone(string phone);

    Task<List<Tasks>> GetTaskOfUser(string userId, DateOnly date);

    Task<bool> AcceptHome(string userId, string homeId);

    Task<List<Home>> GetHomeByUserId(string userId);

    Task<bool> OutHome(string userId, string homeId);

    Task<bool> AddRefreshToken(RefreshToken refreshToken);

    Task<RefreshToken> GetRefreshToken(string refreshToken);

    Task<bool?> UpdateImageUser(string imagePath, string id);
}