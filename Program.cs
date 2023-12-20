using MobileBackend.IResponsitory;
using MobileBackend.Services;
using MobileBackend.IService;
using MobileBackend.Responsitories;
using MobileBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                      });
});

// JWT
var secretKey = builder.Configuration["AppSettings:SecretKey"] ?? "";
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // tu cap token
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        //ký vào token
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                        ClockSkew = TimeSpan.Zero
                    };
                });


builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// mobgodb Config
builder.Services.Configure<MongoDbSetting>(builder.Configuration.GetSection("MongoDB"));

var CerPath = builder.Configuration["CerPath"] ?? "";
var CertPassword = builder.Configuration["CerPassword"];

// kestrel config
//builder.WebHost.ConfigureKestrel((context, serverOptions) =>
//{

//    var host = Dns.GetHostEntry("smartapp.io");
//    serverOptions.Listen(host.AddressList[0], 5000);
//    serverOptions.Listen(host.AddressList[0], 5001, listenOptions =>
//    {
//        listenOptions.UseHttps(CerPath, CertPassword);
//    });

//    serverOptions.ConfigureHttpsDefaults(listenOptions =>
//    {
//        listenOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
//    });
//});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserResponsitory>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IHomeResponsitory, HomeResponsitory>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskResponsitory, TaskResponsitory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseCors(MyAllowSpecificOrigins);
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot\\upload\\images")),
    RequestPath = "/resources"
});

app.MapControllers();

app.Run();
