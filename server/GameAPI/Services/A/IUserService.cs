using GameAPI.Models;
using GameAPI.NewFolder;

namespace GameAPI.Services;

public interface IUserService
{
    Task<List<GameUser>> GetAllUsersAsync();
    Task<GameUser?> GetUserByIdAsync(int Id);
    Task<GameUser> CreateUserAsync(CreateUserDto user);

    Task<TokenResponseDto> LoginUserAsync(LoginUserDto reqUser);

    Task<TokenResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request);
}
