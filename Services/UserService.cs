using MobileBackend.IResponsitory;
using MobileBackend.IService;
using MobileBackend.Models;
using MobileBackend.Models.Home;
using MobileBackend.Models.Task;
using MobileBackend.Models.User;
using static System.Net.WebRequestMethods;


namespace MobileBackend.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository IUserRepo)
    {
        _userRepo = IUserRepo;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _userRepo.GetAllUsers();
    }

    public async Task<User> Login(UserLogin userLogin)
    {
        userLogin.Password = CreateMD5(userLogin.Password);
        var user = await _userRepo.Login(userLogin);
        return user;
    }

    public async Task<User> GetUserById(string userId)
    {
        var user = await _userRepo.GetUserById(userId);
        return user;
    }

    public async Task<bool> UpdateUser(string userId, UserInfor user)
    {
        try
        {
            return await _userRepo.UpdateUser(userId, user);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> Register(UserRegister newUser)
    {
        int totalUser = this.GetAllUsers().Result.Count;

        if (totalUser < 9999)
        {
            newUser.UserId = (totalUser + 1).ToString().PadLeft(5, '0');
        }
        else newUser.UserId = (totalUser + 1).ToString();

        var user =
            new User(
                newUser.UserId,
                newUser.Name,
                null,
                null,
                new Contact(null, newUser.Phone),
                CreateMD5(newUser.Password),
                0,
                null,
                new List<string>(),
                DateTime.Now,
                null
            );

        if (await _userRepo.Register(user))
        {
            return true;
        }
        return false;
    }


    public async Task<User> GetUserByPhone(string phone)
    {
        return await _userRepo.GetUserByPhone(phone);
    }

    public async Task<List<Tasks>> GetTaskOfUser(string userId, DateOnly date)
    {
        return await _userRepo.GetTaskOfUser(userId, date);
    }


    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes); // .NET 5 +
        }
    }

    public async Task<bool> AcceptHome(string userId, string homeId)
    {
        return await _userRepo.AcceptHome(userId, homeId);
    }

    public async Task<List<Home>> GetHomeByUserId(string userId)
    {
        return await _userRepo.GetHomeByUserId(userId);
    }

    public async Task<bool> OutHome(string userId, string homeId)
    {
        return await _userRepo.OutHome(userId, homeId);
    }

    public async Task<bool> AddRefreshToken(RefreshToken refreshToken)
    {
        return await _userRepo.AddRefreshToken(refreshToken);
    }

    public async Task<RefreshToken> GetRefreshToken(string refreshToken)
    {
        return await _userRepo.GetRefreshToken(refreshToken);
    }

    public async Task<bool?> UpdateImageUser(string imagePath, string id)
    {
        return await _userRepo.UpdateImageUser(imagePath, id);
    }
}