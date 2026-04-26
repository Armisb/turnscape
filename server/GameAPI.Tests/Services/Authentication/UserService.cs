using GameAPI.Data;
using GameAPI.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using shared_lib;
using GameAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using GameAPI.Services.User;

namespace GameAPI.Tests;

public class UserServiceTests : IDisposable
{
    AppDbContext dbContext;
    IConfiguration configuration;
    JwtAuthenticationService authService;
    UserService userService;
    private readonly ITestOutputHelper output;

    public UserServiceTests(ITestOutputHelper output)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        dbContext = new AppDbContext(optionsBuilder.Options);

        configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

        authService = new JwtAuthenticationService(dbContext, configuration);
        userService = new UserService(dbContext, configuration);

        this.output = output;
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }

    [Fact]
    public async Task GetAllUsersFromNonEmptyDb()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };

        GameUser gameUser = await authService.SignupUserAsync(createUserDto);
        createUserDto.UserName = "Test1";
        createUserDto.Password = "Test";
        GameUser gameUser2 = await authService.SignupUserAsync(createUserDto);

        List<GameUser> gameUsers = await userService.GetAllUsersAsync();
        Assert.Equal(2, gameUsers.Count);
        Assert.NotNull(gameUsers.Find(x => x.Id == gameUser.Id));
        Assert.NotNull(gameUsers.Find(x => x.Id == gameUser2.Id));
    }

    [Fact]
    public async Task GetAllUsersFromEmptyDb()
    {
        List<GameUser> gameUsers = await userService.GetAllUsersAsync();
        Assert.Empty(gameUsers);
    }
}