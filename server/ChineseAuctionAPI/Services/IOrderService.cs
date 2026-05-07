using ChineseAuctionAPI.DTOs;

namespace ChineseAuctionAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllAsync(int userId);
        Task<bool> DeleteAsync(int orderId, int giftId, int amount);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> DeleteGiftFromOrderAsync(int orderId, int giftId);
        Task<OrderDTO?> GetDraftOrderByUserAsync(int userId);
        Task<bool> AddOrUpdateGiftInOrderAsync(int userId, int giftId, int deltaAmount);
        Task<OrderDTO?> GetByIdWithGiftsAsync(int orderId);
        Task<bool> CompleteOrder(int orderId);
        Task<bool> UpdatePackageQuantityAsync(int userId, int packageId, int amount);
        Task<IncomeReportDTO> GetIncomeReportAsync();
        Task<bool> DeleteDraftOrderByUserAsync(int userId);
        Task<bool> DeleteGiftFromDraftByUserAsync(int userId, int giftId);

    }
}