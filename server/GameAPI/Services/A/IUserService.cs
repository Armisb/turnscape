using GameAPI.Models;

namespace GameAPI.Services;

public interface IUserService
{
    Task<List<GameUser>> GetAllUsersAsync();
    Task<GameUser> GetAllUserByIdAsync(int Id);
    Task<GameUser> CreateUserAsync();

}
