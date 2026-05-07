namespace ChineseAuctionAPI.Services
{
    using ChineseAuctionAPI.DTOs;

    public interface IGiftCategoryService
    {
        Task<List<GiftCategoryDTO>> GetAllAsync();
        Task<GiftCategoryDTO?> GetByIdAsync(int id);
        Task<GiftCategoryDTO> CreateAsync(CreateGiftCategoryDTO dto);
        Task<bool> UpdateAsync(int id, UpdateGiftCategoryDTO dto);
        Task<bool> DeleteAsync(int id);
    }

}
