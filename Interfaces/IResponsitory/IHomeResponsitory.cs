using MobileBackend.Models.Home;
using MongoDB.Bson;

namespace MobileBackend.IResponsitory;

public interface IHomeResponsitory
{
    Task<List<Home>> GetAll();

    Task<HomeWithUser> GetById(string homeId);

    Task<bool> Create(Home home);

    Task<bool> AddMember(string homeId, Member member);

    Task<bool> DeleteMember(string homeId, string userId);

    Task<bool> UpdateHome(string homeId, HomeInfor homeInfor);

    Task<bool> DeleteHome(string homeId);


}