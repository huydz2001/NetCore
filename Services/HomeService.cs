using System.Diagnostics.Eventing.Reader;
using MobileBackend.IResponsitory;
using MobileBackend.IService;
using MobileBackend.Models.Home;
using MongoDB.Bson;


namespace MobileBackend.Services;

public class HomeService : IHomeService
{
    private readonly IHomeResponsitory _homeRepo;

    public HomeService(IHomeResponsitory homeRepo)
    {
        _homeRepo = homeRepo;
    }

    public async Task<HomeWithUser> GetById(string homeId)
    {
        var homes = await _homeRepo.GetById(homeId);
        return homes;
    }

    public async Task<bool> Create(Home home)
    {
        return await _homeRepo.Create(home);
    }

    public async Task<List<Home>> GetAll()
    {
        return await _homeRepo.GetAll();
    }

    public async Task<bool> UpdateHome(string homeId, HomeInfor homeInfor)
    {
        return await _homeRepo.UpdateHome(homeId, homeInfor);
    }

    public async Task<bool> AddMember(string homeId, Member member)
    {
        return await _homeRepo.AddMember(homeId, member);
    }


    public async Task<bool> DeleteMember(string homeId, string userId)
    {
        return await _homeRepo.DeleteMember(homeId, userId);
    }

    public async Task<bool> DeleteHome(string homeId)
    {
        return await _homeRepo.DeleteHome(homeId);
    }

    

}