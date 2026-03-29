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

        configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

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
        var createUserDto = new CreateUserDto
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
        var createUserDto = new CreateUserDto
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
        var createUserDto = new CreateUserDto
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

    [Fact]
    public async Task TryToLoginToEmptyDb()
    {
        var loginUserDto = new LoginUserDto
        {
            UserName = "Test",
            Password = "Test"
        }; 
        var exception = await Record.ExceptionAsync(async () => {
            await authService.LoginUserAsync(loginUserDto);
        });
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("User does not exist", exception.Message);
    }

    [Fact]
    public async Task TryToLoginToNonEmptyDbWithNonExistentUserName()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        await authService.SignupUserAsync(createUserDto);

        var loginUserDto = new LoginUserDto
        {
            UserName = "Test1",
            Password = "Test"
        }; 
        var exception = await Record.ExceptionAsync(async () => {
            await authService.LoginUserAsync(loginUserDto);
        });
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("User does not exist", exception.Message);
    }

    [Fact]
    public async Task TryToLoginWithCorrectCredentials()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        await authService.SignupUserAsync(createUserDto);

        var loginUserDto = new LoginUserDto
        {
            UserName = "Test",
            Password = "Test"
        }; 
        TokenResponseDto tokenResponseDto = await authService.LoginUserAsync(loginUserDto);

        Assert.NotNull(tokenResponseDto.AccessToken);
        Assert.NotNull(tokenResponseDto.RefreshToken);
        GameUser? gameUser = await dbContext.GameUsers.FirstOrDefaultAsync(u => u.UserName == loginUserDto.UserName);
        Assert.NotNull(gameUser);
        Assert.Equal(gameUser.RefreshToken, tokenResponseDto.RefreshToken);
    }
    
    [Fact]
    public async Task TryToLoginWithIncorrectCredentials()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        await authService.SignupUserAsync(createUserDto);

        var loginUserDto = new LoginUserDto
        {
            UserName = "Test",
            Password = "Test2"
        }; 
        var exception = await Record.ExceptionAsync(async () => {
            await authService.LoginUserAsync(loginUserDto);
        });
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("Bad password", exception.Message);
    }

    [Fact]
    public async Task RefreshTokenWithValidToken()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var loginUserDto = new LoginUserDto
        {
            UserName = "Test",
            Password = "Test"
        }; 
        TokenResponseDto tokenResponseDto = await authService.LoginUserAsync(loginUserDto);

        var refreshTokenRequestDto = new RefreshTokenRequestDto
        {
            Id = gameUser.Id,
            RefreshToken = tokenResponseDto.RefreshToken
        };

        TokenResponseDto? tokenResponseDto1 = await authService.RefreshTokensAsync(refreshTokenRequestDto);
        Assert.NotNull(tokenResponseDto1);
        Assert.NotNull(tokenResponseDto1.AccessToken);
        Assert.NotNull(tokenResponseDto1.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenWhenUserDoesNotExist()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var loginUserDto = new LoginUserDto
        {
            UserName = "Test",
            Password = "Test"
        }; 
        TokenResponseDto tokenResponseDto = await authService.LoginUserAsync(loginUserDto);

        var refreshTokenRequestDto = new RefreshTokenRequestDto
        {
            Id = new Guid(),
            RefreshToken = tokenResponseDto.RefreshToken
        };

        TokenResponseDto? tokenResponseDto1 = await authService.RefreshTokensAsync(refreshTokenRequestDto);
        Assert.Null(tokenResponseDto1);
    }

    [Fact]
    public async Task RefreshTokenWithInvalidToken()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var refreshTokenRequestDto = new RefreshTokenRequestDto
        {
            Id = gameUser.Id,
            RefreshToken = "invalid"
        };

        TokenResponseDto? tokenResponseDto1 = await authService.RefreshTokensAsync(refreshTokenRequestDto);
        Assert.Null(tokenResponseDto1);
    }

    [Fact]
    public async Task RefreshTokenWithExpiredToken()
    {
        var createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var loginUserDto = new LoginUserDto
        {
            UserName = "Test",
            Password = "Test"
        }; 
        TokenResponseDto tokenResponseDto = await authService.LoginUserAsync(loginUserDto);

        var refreshTokenRequestDto = new RefreshTokenRequestDto
        {
            Id = gameUser.Id,
            RefreshToken = tokenResponseDto.RefreshToken
        };

        var user = dbContext.GameUsers.First(u => u.Id.Equals(gameUser.Id));
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1);
        dbContext.SaveChanges();

        TokenResponseDto? tokenResponseDto1 = await authService.RefreshTokensAsync(refreshTokenRequestDto);
        Assert.Null(tokenResponseDto1);
    }
}
