using System.Reflection;
using ChineseAuctionAPI.Data;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Repositories
{
    public interface IGiftRepo
    {
        Task<IEnumerable<Gift>> GetAllAsync();
        Task<Gift?> GetByIdAsync(int id);
        Task<Gift> AddAsync(Gift gift);
        Task<bool> UpdateAsync(Gift gift);
        Task<bool> DeleteAsync(int id);
        Task<Gift?> GetGiftWithOrdersAndUsersAsync(int giftId);
        Task AddWinnerAsync(Winner winner);
        Task<IEnumerable<Winner>> GetAllWinnersAsync();
        Task<IEnumerable<Gift?>> GetByNameDonor(string donor);
        Task<IEnumerable<Gift?> >GetByNumOfBuyers(int buyers);
        Task<IEnumerable<Gift?>> GetByNameGift(string word);
        Task<IEnumerable<Gift?>> SortByAmountPeople();
    }
}
