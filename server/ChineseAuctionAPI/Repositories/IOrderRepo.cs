using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Repositories
{
    public interface IOrderRepo
    {
        Task<bool> DeleteAsync(int orderId, int giftId, int amount);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> DeleteGiftFromOrderAsync(int orderId, int giftId);
        Task<Order?> GetDraftOrderByUserAsync(int userId);

        Task<Order> CreateDraftOrderAsync(int userId);

        Task AddOrUpdateGiftInOrderAsync(int userId, int giftId, int deltaAmount);

        Task<Order?> GetByIdWithGiftsAsync(int orderId);

        Task<bool?> CompleteOrder(int orderId);

        Task <IEnumerable <Order?> > GetAllOrder(int userId);
        Task AddOrUpdatePackageInOrderAsync(int userId, int packageId, int amount);
        Task<IncomeReportDTO> GetIncomeReportAsync();


    }
}