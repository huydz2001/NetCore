using System;
using System.Diagnostics.Eventing.Reader;
using MobileBackend.IResponsitory;
using MobileBackend.IService;
using MobileBackend.Models.Home;
using MongoDB.Bson;


namespace MobileBackend.Services;

public class HomeService : IHomeService
{
    private readonly IHomeResponsitory _homeRepo;
    private readonly IWebHostEnvironment environment;

    public HomeService(IHomeResponsitory homeRepo, IWebHostEnvironment environment)
    {
        this.environment = environment;
        _homeRepo = homeRepo;
    }

    public async Task<HomeWithUser> GetById(string homeId)
    {
        var homes = await _homeRepo.GetById(homeId);
        return homes;
    }

    public async Task<bool> Create(HomeCreate? homeCreate, IFormFile? fileImage)
    {
        int totalHome = this.GetAll().Result.Count;
        string homeId = string.Empty;
        string img = string.Empty;

        if (totalHome < 9999)
        {
            homeId = (totalHome + 1).ToString().PadLeft(5, '0');
        }
        else homeId = (totalHome + 1).ToString();

        if (fileImage?.Length > 0)
        {
            string filePath = environment.WebRootPath + "\\upload\\images\\homes\\" + homeId;
            string imagePath = filePath + "\\" + fileImage.FileName.Split('.')[0] + DateTime.Now.Millisecond + "." + fileImage.FileName.Split('.')[1];

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            string[] parts = imagePath.Split('\\');
            string[] lastTwoParts = parts.Skip(parts.Length - 2).ToArray();
            img = string.Join("/", lastTwoParts);



            using (FileStream stream = System.IO.File.Create(imagePath))
            {
                await fileImage.CopyToAsync(stream);
            }
        }
        else
        {
            img = string.Empty;
        }


        Home home = new Home(homeId, homeCreate?.Name, img, homeCreate?.Slogan, homeCreate?.Members, homeCreate?.CreatedBy, DateTime.Now);

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