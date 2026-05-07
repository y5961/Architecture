using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Repositories
{
    public interface IPackageRepo
    {
        Task<IEnumerable<Package>> GetAllAsync();
        Task<Package?> GetByIdAsync(int id);
        Task<int> AddAsync(Package package);
        Task UpdateAsync(Package package);
        Task DeleteAsync(int id);
    }
}