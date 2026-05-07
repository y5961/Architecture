using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<DtoUserOrder?> GetUserWithOrdersAsync(int userId);
        Task<string> RegisterAsync(DtoLogin dto);
        Task<string> LoginAsync(string email, string password);

    }
}
