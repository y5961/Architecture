using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Repositories
{
    public interface IUserRepo
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> AddAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistEmailAsync(string email);
        Task<User?> GetUserWithOrdersAsync(int userId);
    }
}
