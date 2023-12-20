using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MobileBackend.IService;
using MobileBackend.Models;
using MobileBackend.Models.Home;
using MobileBackend.Models.Task;
using MobileBackend.Models.User;
using MongoDB.Bson;
using ThirdParty.Json.LitJson;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace MobileBackend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppSettings _appSettings;

    private readonly IWebHostEnvironment environment;
    public UserController(IUserService userService, IWebHostEnvironment environment, IOptionsMonitor<AppSettings> optionsMonitor)
    {
        this.environment = environment;
        _userService = userService;
        _appSettings = optionsMonitor.CurrentValue;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        try
        {
            List<User> users = await _userService.GetAllUsers();
            if (users.Count > 0)
            {
                return Ok(users);
            }
            return StatusCode(200, "Don't have any user");
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(UserLogin userLogin)
    {
        try
        {
            User user = await _userService.Login(userLogin);
            if (user != null)
            {
                var token = await GenerateToken(user);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = token

                });
            }
            return BadRequest("Login failed");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(string id)
    {
        try
        {

            User user = await _userService.GetUserById(id);
            if (user != null)
            {
                return Ok(user);
            }
            return StatusCode(200, "Not found user by id: " + id);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(UserRegister user)
    {
        try
        {
            if (await _userService.Register(user))
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success"
                });
            }
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Has an error while registing"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }


    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> UpdateById(string id, [FromForm] string jsonData, IFormFile fileImage)
    {
        try
        {
            var user = JsonConvert.DeserializeObject<UserInfor>(jsonData);
            
            User filter = await _userService.GetUserById(id);
            if (filter == null)
            {
                return BadRequest("No user with id = " + id);
            }
            else
            {
                
                await _userService.UpdateUser(id, user!);
                return Ok("Update Success");
            }
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("{id}/uploadImage")]
    [Authorize]
    public async Task<IActionResult> UploadImage(string id,IFormFile fileImage)
    {
        try
        {
            if (fileImage.Length > 0)
            {
                string filePath = environment.WebRootPath + "\\upload\\images\\" + id ;
                string imagePath = filePath + "\\" + fileImage.FileName.Split('.')[0] + DateTime.Now.Millisecond+ "." + fileImage.FileName.Split('.')[1];

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
                string result = string.Join("/", lastTwoParts);

                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await fileImage.CopyToAsync(stream);
                    await _userService.UpdateImageUser(result, id);
                    return Ok(result);
                }
            }
            else
            {
                return Ok("Not image upload");
            }
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage1(IFormFile fileImage)
    {
        try
        {
            if (fileImage.Length > 0)
            {
                string filePath = environment.WebRootPath + "\\upload\\images";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(filePath);
                    FileInfo[] files = directory.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }

                string imagePath = filePath + "\\" + fileImage.FileName;

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }


                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await fileImage.CopyToAsync(stream);
                    return Ok("images" + fileImage.FileName);
                }
            }
            else
            {
                return Ok("Not image upload");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{phone}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUserByPhone(string phone)
    {
        try
        {

            User user = await _userService.GetUserByPhone(phone);
            if (user != null)
            {
                return Ok(user);
            }
            return StatusCode(200, "Not found user by phone: " + phone);
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpPatch("{id}/acceptJoinHome")]
    [Authorize]
    public async Task<ActionResult<bool>> AcceptHome(string id, string homeId)
    {
        try
        {
            if (await _userService.AcceptHome(id, homeId))
            {
                return Ok("Join Success");
            }
            return BadRequest("Join Failed");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }


    [HttpGet("{id}/getHome")]
    [Authorize]
    public async Task<ActionResult<List<Home>>> GetHome(string id)
    {
        try
        {
            var result = await _userService.GetHomeByUserId(id);
            if (result.Count > 0)
            {
                return Ok(result);
            }
            return Ok("UserId " + id + " does not belong to any home");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpGet("{id}/getTask")]
    [Authorize]
    public async Task<ActionResult<List<Home>>> GetTasks(string id, DateOnly date)
    {
        try
        {
            var result = await _userService.GetTaskOfUser(id, date);
            if (result.Count > 0)
            {
                return Ok(result);
            }
            return Ok("Don't have any task");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpDelete("{id}/outHome")]
    [Authorize]
    public async Task<ActionResult<List<Home>>> OutHome(string id, string homeId)
    {
        try
        {
            var result = await _userService.OutHome(id, homeId);
            if (result)
            {
                return Ok("Out Success");
            }
            return BadRequest("Out failed");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    private async Task<Token> GenerateToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("Image", user?.Image?.ToString() ?? ""),
                }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescription);
        var accessToken = jwtTokenHandler.WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = new ObjectId(),
            JwtId = token.Id,
            UserId = user!.UserId,
            Token = refreshToken,
            IsUsed = false,
            IsRevoked = false,
            IssuedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddDays(1)
        };

        await _userService.AddRefreshToken(refreshTokenEntity);

        return new Token
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

    }

    [HttpPost("RenewToken")]
    public async Task<IActionResult> RenewToken(Token model)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
        var tokenValidateParam = new TokenValidationParameters
        {
            //tự cấp token
            ValidateIssuer = false,
            ValidateAudience = false,

            //ký vào token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

            ClockSkew = TimeSpan.Zero,

            ValidateLifetime = false //ko kiểm tra token hết hạn
        };
        try
        {
            //check 1: AccessToken valid format
            var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

            //check 2: Check alg
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                if (!result)//false
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid token"
                    });
                }
            }

            //check 3: Check accessToken expire?
            var exprise = tokenInVerification?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value ?? "";
            var utcExpireDate = long.Parse(exprise);

            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
            if (expireDate > DateTime.UtcNow)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Access token has not yet expired"
                });
            }

            //check 4: Check refreshtoken exist in DB
            var storedToken = await _userService.GetRefreshToken(model.RefreshToken);
            if (storedToken == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token does not exist"
                });
            }

            //check 5: check refreshToken is used/revoked?
            if (storedToken.IsUsed)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token has been used"
                });
            }
            if (storedToken.IsRevoked)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token has been revoked"
                });
            }

            //check 6: AccessToken id == JwtId in RefreshToken
            var jti = tokenInVerification?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (storedToken.JwtId != jti)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Token doesn't match"
                });
            }

            //Update token is used
            storedToken.IsRevoked = true;
            storedToken.IsUsed = true;


            //create new token
            var user = await _userService.GetUserById(storedToken.UserId);
            var token = await GenerateToken(user);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Renew token success",
                Data = token
            });
        }
        catch (Exception)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Something went wrong"
            });
        }
    }

    private string GenerateRefreshToken()
    {
        var random = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);

            return Convert.ToBase64String(random);
        }
    }

    private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
    {
        var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

        return dateTimeInterval;
    }
}