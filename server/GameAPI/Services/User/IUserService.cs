using GameAPI.Models;
using shared_lib;

namespace GameAPI.Services;

public interface IUserService
{
    Task<List<GameUser>> GetAllUsersAsync();
}
