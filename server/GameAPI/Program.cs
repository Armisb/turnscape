using GameAPI.Data;
using GameAPI.Services;
using GameAPI.Services.User;
using GameAPI.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GameAPI.Services.ItemType;
using GameAPI.Services.Item;
using GameAPI.Services.Lobby;
using Microsoft.AspNetCore.SignalR;
using GameAPI.Hubs;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

// Authentication setup and configurations
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero,

        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"])),
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500") // client origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});
builder.Services.AddSingleton<IUserIdProvider, QueryUserIdProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
builder.Services.AddScoped<IItemTypeService, ItemTypeService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ILobbyService, LobbyService>();
builder.Services.AddHostedService<MatchFinder>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();
app.UseCors();
app.MapHub<GameAPI.Hubs.MatchHub>("/matchhub");

app.Run();
