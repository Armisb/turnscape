using GameAPI.Data;
using GameAPI.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using shared_lib;
using GameAPI.Models;

namespace GameAPI.Tests;

public class AuthenticationServiceTests : IDisposable
{
    AppDbContext dbContext;
    IConfiguration configuration;
    JwtAuthenticationService authService;
    private readonly ITestOutputHelper output;

    public AuthenticationServiceTests(ITestOutputHelper output)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        dbContext = new AppDbContext(optionsBuilder.Options);

        configuration = new ConfigurationBuilder().Build();

        authService = new JwtAuthenticationService(dbContext, configuration);

        this.output = output;
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }

    [Fact]
    public async Task SignupUserInEmptyDb()
    {
        Assert.Equal(0, dbContext.GameUsers.Count());
        CreateUserDto createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        Assert.Equal(createUserDto.UserName, gameUser.UserName);
        Assert.NotNull(gameUser.PasswordHash);
        Assert.Equal(1, dbContext.GameUsers.Count());
        Assert.True(dbContext.GameUsers.Contains(gameUser));
    }

    [Fact]
    public async Task SignupUserInNonEmptyDb()
    {
        CreateUserDto createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        await authService.SignupUserAsync(createUserDto);

        Assert.Equal(1, dbContext.GameUsers.Count());

        createUserDto.UserName = "Test2";
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        Assert.Equal(createUserDto.UserName, gameUser.UserName);
        Assert.Equal(2, dbContext.GameUsers.Count());
        Assert.True(dbContext.GameUsers.Contains(gameUser));
    }

    [Fact]
    public async Task SignupUserWhenUsernameIsTaken()
    {
        CreateUserDto createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        await authService.SignupUserAsync(createUserDto);

        var exception = await Record.ExceptionAsync(async () => {
            await authService.SignupUserAsync(createUserDto);
        });
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("Username is taken", exception.Message);
    }
}
