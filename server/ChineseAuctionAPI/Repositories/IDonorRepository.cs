using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Repositories
{
    public interface IDonorRepository
    {
        Task<IEnumerable<Donor>> GetAllAsync();
        Task<Donor> GetByIdAsync(int id);
        Task<int> AddAsync(Donor donor);
        Task UpdateAsync(Donor donor);
        Task DeleteAsync(int id);
        Task<IEnumerable<Gift>>? GetGiftsAsync(int DonorId);
        Task<Donor> SortByGift(string donor);
        Task<IEnumerable<Donor?>> SortByEmail(string email);
        Task<IEnumerable<Donor?>> SortByName(string name);
    }
}