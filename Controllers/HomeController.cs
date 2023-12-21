using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileBackend.IService;
using MobileBackend.Models.Home;
using MobileBackend.Models.User;
using MobileBackend.Services;
using Newtonsoft.Json;
using System;

namespace MobileBackend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;


    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;

    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<Home>>> GetAll()
    {
        try
        {
            List<Home> homes = await _homeService.GetAll();
            if (homes.Count > 0)
            {
                return Ok(homes);
            }
            return StatusCode(200, "Don't have any home");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<HomeWithUser> GetById(string id)
    {
        return await _homeService.GetById(id);
    }

    [HttpPut("{id}/update")]
    [Authorize]
    public async Task<ActionResult> UpdateHome(string id, HomeInfor homeInfor)
    {
        try
        {
            if (await _homeService.UpdateHome(id, homeInfor))
            {
                return Ok("Update success");
            }
            return BadRequest("Update success");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }


    [HttpPost("{id}/addMember")]
    [Authorize]
    public async Task<ActionResult<Home>> AddMember(string id, Member member)
    {
        try
        {
            if (await _homeService.AddMember(id, member))
            {
                return Ok("Add success");
            }
            return NotFound("Add failed");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(400, ex.Message);
        }
    }

    [HttpDelete("{id}/deleteMember")]
    [Authorize]
    public async Task<ActionResult> DeleteMember(string id, string userId)
    {
        try
        {
            if (await _homeService.DeleteMember(id, userId))
            {
                return Ok("Delete success");
            }
            return BadRequest("Delete success");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteHome(string id)
    {
        try
        {
            if (await _homeService.DeleteHome(id))
            {
                return Ok("Delete Success");
            }
            return BadRequest("Delete Failed");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpPost("addWithImage")]
    [Authorize]
    public async Task<ActionResult> AddHomeWithImage([FromForm] string jsonData, IFormFile? fileImage)
    {
        try
        {
            HomeCreate? home = JsonConvert.DeserializeObject<HomeCreate>(jsonData);

            if(await _homeService.Create(home, fileImage))
            {
                return Ok("success");
            }
            return BadRequest();
            

        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }






}