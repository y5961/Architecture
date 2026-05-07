using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _OrderRepository;
        private readonly ILogger<OrderService> _logger;
        private readonly IConfiguration _config;

        public OrderService(IOrderRepo OrderRepository, IConfiguration config, ILogger<OrderService> logger)
        {
            _OrderRepository = OrderRepository;
            _logger = logger;
            _config = config;
        }

        public async Task<bool> AddOrUpdateGiftInOrderAsync(int userId, int giftId, int deltaAmount)
        {
            try
            {
                _logger.LogInformation("מעדכן/מוסיף מתנה {GiftId} למשתמש {UserId} בדלתא {DeltaAmount}.", giftId, userId, deltaAmount);
                await _OrderRepository.AddOrUpdateGiftInOrderAsync(userId, giftId, deltaAmount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בעת עדכון מתנה {GiftId} בהזמנה של משתמש {UserId}.", giftId, userId);
                throw;
            }
        }

        public async Task<bool> CompleteOrder(int orderId)
        {
            try
            {
                _logger.LogInformation("מנסה לסגור (Complete) הזמנה שמספרה {OrderId}.", orderId);
                await _OrderRepository.CompleteOrder(orderId);
                _logger.LogInformation("הזמנה {OrderId} נסגרה בהצלחה.", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "נכשלה סגירת הזמנה {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int orderId, int giftId, int amount)
        {
            try
            {
                _logger.LogInformation("מוחק {Amount} יחידות של מתנה {GiftId} מהזמנה {OrderId}.", amount, giftId, orderId);
                var result = await _OrderRepository.DeleteAsync(orderId, giftId, amount);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה במחיקת מתנה מהזמנה {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderDTO>> GetAllAsync(int userId)
        {
            try
            {
                _logger.LogInformation("שולף את כל ההזמנות עבור משתמש {UserId}.", userId);
                var orders = await _OrderRepository.GetAllOrder(userId);

                var ordersDto = orders.Select(o => new OrderDTO
                {
                    IdOrder = o.IdOrder, // התיקון הקריטי כאן!
                    IdUser = o.IdUser,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.OrdersGift.Sum(og => og.Amount),
                    //TotalPrice = o.OrdersGift.Sum(og => og.Amount * og.Gift.Price),
                    OrdersGifts = o.OrdersGift.Select(og => new OrdersGiftDTO
                    {
                        Category = og.Gift.Category?.Name ?? "",
                        Name = og.Gift.Name ?? "",
                        Amount = og.Amount,
                        Description = og.Gift.Description,
                        Image = og.Gift.Image
                    }).ToList()
                    ,
                    OrdersPackages = o.OrdersPackage.Select(op => new OrdersPackageDTO
                    {
                        IdPackage = op.IdPackage,
                        Quantity = op.Quantity
                    }).ToList()
                }).ToList();

                return ordersDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשליפת הזמנות למשתמש {UserId}.", userId);
                throw;
            }
        }

        public async Task<OrderDTO?> GetByIdWithGiftsAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("שולף פרטי הזמנה {OrderId} כולל פירוט מתנות.", orderId);
                var order = await _OrderRepository.GetByIdWithGiftsAsync(orderId);

                if (order == null)
                {
                    _logger.LogWarning("הזמנה {OrderId} לא נמצאה.", orderId);
                    return null;
                }

                return new OrderDTO
                {
                    IdOrder = order.IdOrder, // תיקון: הוספת ה-ID שחסר
                    IdUser = order.IdUser,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    OrdersGifts = order.OrdersGift.Select(og => new OrdersGiftDTO
                    {
                        Name = og.Gift.Name,
                        Description = og.Gift.Description,
                        Category = og.Gift.Category?.Name ?? "",
                        Amount = og.Amount,
                        Image = og.Gift.Image
                    }).ToList(),
                    TotalAmount = order.OrdersGift.Sum(og => og.Amount),
                    //TotalPrice = order.OrdersGift.Sum(og => og.Amount * og.Gift.Price)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשליפת הזמנה {OrderId} עם מתנות.", orderId);
                throw;
            }
        }
        public async Task<OrderDTO?> GetDraftOrderByUserAsync(int userId)
        {
            try
            {
                _logger.LogInformation("מחפש הזמנה במצב טיוטה (Draft) עבור משתמש {UserId}.", userId);
                var order = await _OrderRepository.GetDraftOrderByUserAsync(userId);

                if (order == null) return null;

                // כאן התיקון הקריטי - את חייבת להעתיק את הערכים מה-Model ל-DTO
                return new OrderDTO
                {
                    IdOrder = order.IdOrder, // זה השורה שהייתה חסרה וגרמה ל-0!
                    IdUser = order.IdUser,
                    OrderDate = order.OrderDate,
                    Status = order.Status,

                    // חישוב סכומים בזמן אמת כדי שלא יחזור 0 ב-Angular
                    TotalAmount = order.OrdersGift?.Sum(og => og.Amount) ?? 0,
                    //TotalPrice = order.OrdersGift?.Sum(og => (og.Amount) * (og.Gift?.Price ?? 0)) ?? 0,

                    OrdersGifts = order.OrdersGift?.Select(og => new OrdersGiftDTO
                    {
                        Name = og.Gift?.Name ?? "ללא שם",
                        Amount = og.Amount,
                        Description = og.Gift?.Description,
                        Category = og.Gift?.Category?.Name ?? "",
                        Image = og.Gift?.Image
                    }).ToList() ?? new List<OrdersGiftDTO>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשליפת טיוטת הזמנה למשתמש {UserId}.", userId);
                throw;
            }
        }
     
        public async Task<bool> UpdatePackageQuantityAsync(int userId, int packageId, int amount)
        {
            try
            {
                _logger.LogInformation("Updating package {PackageId} for user {UserId} with amount {Amount}", packageId, userId, amount);
                await _OrderRepository.AddOrUpdatePackageInOrderAsync(userId, packageId, amount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package in order for user {UserId}", userId);
                return false;
            }
        }
        public async Task<IncomeReportDTO> GetIncomeReportAsync()
        {
            try
            {
                _logger.LogInformation("ח הכנסות כללי למערכת.");
                return await _OrderRepository.GetIncomeReportAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בהפקת דוח הכנסות.");
        
                throw;
            }
        }
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("מוחק הזמנה {OrderId} כולל פריטים.", orderId);
                var result = await _OrderRepository.DeleteOrderAsync(orderId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה במחיקת הזמנה {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<bool> DeleteGiftFromOrderAsync(int orderId, int giftId)
        {
            try
            {
                _logger.LogInformation("מוחק מתנה {GiftId} מהזמנה {OrderId}.", giftId, orderId);
                var result = await _OrderRepository.DeleteGiftFromOrderAsync(orderId, giftId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה במחיקת מתנה {GiftId} מהזמנה {OrderId}.", giftId, orderId);
                throw;
            }
        }

        public async Task<bool> DeleteDraftOrderByUserAsync(int userId)
        {
            try
            {
                var order = await _OrderRepository.GetDraftOrderByUserAsync(userId);
                if (order == null) return false;
                return await _OrderRepository.DeleteOrderAsync(order.IdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה במחיקת טיוטת ההזמנה למשתמש {UserId}.", userId);
                throw;
            }
        }

        public async Task<bool> DeleteGiftFromDraftByUserAsync(int userId, int giftId)
        {
            try
            {
                var order = await _OrderRepository.GetDraftOrderByUserAsync(userId);
                if (order == null) return false;
                return await _OrderRepository.DeleteGiftFromOrderAsync(order.IdOrder, giftId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה במחיקת מתנה {GiftId} מטיוטת ההזמנה של המשתמש {UserId}.", giftId, userId);
                throw;
            }
        }
    }
}