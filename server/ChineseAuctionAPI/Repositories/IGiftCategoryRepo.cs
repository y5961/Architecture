namespace ChineseAuctionAPI.Repositories
{
    using ChineseAuctionAPI.Models;

    public interface IGiftCategoryRepo
    {
        Task<List<GiftCategory>> GetAllAsync();
        Task<GiftCategory?> GetByIdAsync(int id);
        Task<GiftCategory> CreateAsync(GiftCategory category);
        Task UpdateAsync(GiftCategory category);
        Task DeleteAsync(GiftCategory category);
    }

}
