using System.Linq;
using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ChineseAuctionAPI.Repositories
{
    public class MongoOrderRepo : IOrderRepo
    {
        private readonly IMongoCollection<MongoOrderDocument> _orders;
        private readonly IGiftRepo _giftRepo;
        private readonly IPackageRepo _packageRepo;
        private readonly IDonorRepository _donorRepository;

        public MongoOrderRepo(IMongoDatabase database, IGiftRepo giftRepo, IPackageRepo packageRepo, IDonorRepository donorRepository)
        {
            _orders = database.GetCollection<MongoOrderDocument>("orders");
            _giftRepo = giftRepo;
            _packageRepo = packageRepo;
            _donorRepository = donorRepository;
        }

        public async Task AddOrUpdateGiftInOrderAsync(int userId, int IdGift, int deltaAmount)
        {
            var existingOrder = await GetDraftDocumentByUserAsync(userId);
            if (existingOrder == null)
                throw new Exception("עליך לבחור חבילה לפני בחירת מתנות");

            var totalTickets = await CalculateTotalTicketsAsync(existingOrder);
            var currentUsedTickets = existingOrder.OrdersGift.Sum(og => og.Amount);
            var extraRequested = deltaAmount > 0 ? deltaAmount : 0;

            if (deltaAmount > 0 && currentUsedTickets + extraRequested > totalTickets)
            {
                throw new InvalidOperationException("INSUFFICIENT_TICKETS");
            }

            var orderGift = existingOrder.OrdersGift.FirstOrDefault(og => og.IdGift == IdGift);
            if (orderGift != null)
            {
                if (deltaAmount <= 0 && orderGift.Amount + deltaAmount <= 0)
                {
                    existingOrder.OrdersGift.Remove(orderGift);
                }
                else
                {
                    orderGift.Amount += deltaAmount;
                }
            }
            else if (deltaAmount > 0)
            {
                existingOrder.OrdersGift.Add(new MongoOrderGift
                {
                    IdGift = IdGift,
                    Amount = deltaAmount
                });
            }

            await _orders.ReplaceOneAsync(d => d.IdOrder == existingOrder.IdOrder, existingOrder);
        }

        public async Task<bool> DeleteAsync(int orderId, int giftId, int amount)
        {
            var order = await GetDocumentByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            var orderGift = order.OrdersGift.FirstOrDefault(og => og.IdGift == giftId);
            if (orderGift == null)
                throw new Exception("Gift not found in order");

            orderGift.Amount -= amount;
            if (orderGift.Amount <= 0)
            {
                order.OrdersGift.Remove(orderGift);
            }

            await _orders.ReplaceOneAsync(d => d.IdOrder == orderId, order);
            return true;
        }

        public async Task<bool> DeleteGiftFromOrderAsync(int orderId, int giftId)
        {
            var order = await GetDocumentByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            var orderGift = order.OrdersGift.FirstOrDefault(og => og.IdGift == giftId);
            if (orderGift == null)
                return false;

            order.OrdersGift.Remove(orderGift);
            await _orders.ReplaceOneAsync(d => d.IdOrder == orderId, order);
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var result = await _orders.DeleteOneAsync(d => d.IdOrder == orderId);
            return result.DeletedCount > 0;
        }

        public async Task<Order?> GetByIdWithGiftsAsync(int orderId)
        {
            var order = await GetDocumentByIdAsync(orderId);
            return order == null ? null : await MapToOrderAsync(order);
        }

        public async Task<IEnumerable<Order>> GetAllOrder(int userId)
        {
            var docs = await _orders.Find(d => d.IdUser == userId).ToListAsync();
            var results = new List<Order>();
            foreach (var doc in docs)
            {
                var mapped = await MapToOrderAsync(doc);
                if (mapped != null)
                    results.Add(mapped);
            }
            return results;
        }

        public async Task<Order?> GetDraftOrderByUserAsync(int userId)
        {
            var doc = await GetDraftDocumentByUserAsync(userId);
            return doc == null ? null : await MapToOrderAsync(doc);
        }

        public async Task<Order> CreateDraftOrderAsync(int userId)
        {
            var draftOrder = await CreateDraftDocumentAsync(userId);
            return await MapToOrderAsync(draftOrder);
        }

        public async Task AddOrUpdatePackageInOrderAsync(int userId, int packageId, int amount)
        {
            var order = await GetDraftDocumentByUserAsync(userId) ?? await CreateDraftDocumentAsync(userId);

            var package = await _packageRepo.GetByIdAsync(packageId);
            if (package == null)
                throw new Exception("Package not found");

            var orderPackage = order.OrdersPackage.FirstOrDefault(op => op.IdPackage == packageId);
            if (orderPackage != null)
            {
                if (amount <= 0)
                {
                    order.Price -= orderPackage.PriceAtPurchase * orderPackage.Quantity;
                    order.OrdersPackage.Remove(orderPackage);
                }
                else
                {
                    var delta = amount;
                    orderPackage.Quantity += delta;
                    orderPackage.PriceAtPurchase = package.Price;
                    order.Price += package.Price * delta;
                }
            }
            else if (amount > 0)
            {
                order.OrdersPackage.Add(new MongoOrderPackage
                {
                    IdPackage = packageId,
                    Quantity = amount,
                    PriceAtPurchase = package.Price
                });
                order.Price += package.Price * amount;
            }

            var replaceResult = await _orders.ReplaceOneAsync(d => d.IdOrder == order.IdOrder, order, new ReplaceOptions { IsUpsert = true });
            if (replaceResult.MatchedCount == 0 && replaceResult.UpsertedId == null)
            {
                throw new Exception("Failed to persist order package update.");
            }
        }

        public async Task<bool?> CompleteOrder(int orderId)
        {
            var filter = Builders<MongoOrderDocument>.Filter.Eq(d => d.IdOrder, orderId);
            var update = Builders<MongoOrderDocument>.Update.Set(d => d.Status, OrderStatus.Completed);
            var result = await _orders.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0) return null;
            return result.ModifiedCount > 0 || result.MatchedCount > 0;
        }

        public async Task<IncomeReportDTO> GetIncomeReportAsync()
        {
            var completedOrders = await _orders.Find(d => d.Status == OrderStatus.Completed).ToListAsync();
            var totalRevenue = completedOrders.Sum(o => o.Price);
            var totalBuyers = completedOrders.Select(o => o.IdUser).Distinct().Count();
            var totalDonors = (await _donorRepository.GetAllAsync()).Count();

            return new IncomeReportDTO
            {
                TotalRevenue = totalRevenue,
                TotalBuyers = totalBuyers,
                TotalDonors = totalDonors
            };
        }

        private async Task<MongoOrderDocument> CreateDraftDocumentAsync(int userId)
        {
            var nextId = await GetNextOrderIdAsync();
            var draftOrder = new MongoOrderDocument
            {
                IdOrder = nextId,
                IdUser = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Draft,
                Price = 0,
                OrdersGift = new List<MongoOrderGift>(),
                OrdersPackage = new List<MongoOrderPackage>()
            };

            await _orders.InsertOneAsync(draftOrder);
            return draftOrder;
        }

        private async Task<int> GetNextOrderIdAsync()
        {
            var lastOrder = await _orders.Find(Builders<MongoOrderDocument>.Filter.Empty)
                .SortByDescending(d => d.IdOrder)
                .Limit(1)
                .FirstOrDefaultAsync();

            return lastOrder == null ? 1 : lastOrder.IdOrder + 1;
        }

        private async Task<MongoOrderDocument?> GetDraftDocumentByUserAsync(int userId)
        {
            return await _orders.Find(d => d.IdUser == userId && d.Status == OrderStatus.Draft).FirstOrDefaultAsync();
        }

        private async Task<MongoOrderDocument?> GetDocumentByIdAsync(int orderId)
        {
            return await _orders.Find(d => d.IdOrder == orderId).FirstOrDefaultAsync();
        }

        private async Task<int> CalculateTotalTicketsAsync(MongoOrderDocument order)
        {
            var packageData = new List<(int Quantity, int AmountRegular, int AmountPremium)>();
            foreach (var packageLine in order.OrdersPackage)
            {
                var package = await _packageRepo.GetByIdAsync(packageLine.IdPackage);
                if (package != null)
                {
                    packageData.Add((packageLine.Quantity, package.AmountRegular, package.AmountPremium ?? 0));
                }
            }
            return packageData.Sum(x => (x.AmountRegular + x.AmountPremium) * x.Quantity);
        }

        private async Task<Order> MapToOrderAsync(MongoOrderDocument doc)
        {
            var order = new Order
            {
                IdOrder = doc.IdOrder,
                IdUser = doc.IdUser,
                OrderDate = doc.OrderDate,
                Status = doc.Status,
                Price = doc.Price,
                OrdersGift = new List<OrdersGift>(),
                OrdersPackage = new List<OrdersPackage>()
            };

            foreach (var giftLine in doc.OrdersGift)
            {
                var gift = await _giftRepo.GetByIdAsync(giftLine.IdGift);
                var orderGift = new OrdersGift
                {
                    IdGift = giftLine.IdGift,
                    Amount = giftLine.Amount,
                    Gift = gift ?? new Gift { IdGift = giftLine.IdGift },
                    IdOrder = doc.IdOrder,
                    Order = order
                };
                order.OrdersGift.Add(orderGift);
            }

            foreach (var packageLine in doc.OrdersPackage)
            {
                var package = await _packageRepo.GetByIdAsync(packageLine.IdPackage);
                var orderPackage = new OrdersPackage
                {
                    IdPackage = packageLine.IdPackage,
                    Quantity = packageLine.Quantity,
                    PriceAtPurchase = packageLine.PriceAtPurchase,
                    Package = package ?? new Package { IdPackage = packageLine.IdPackage },
                    OrderId = doc.IdOrder,
                    Order = order
                };
                order.OrdersPackage.Add(orderPackage);
            }

            return order;
        }
    }
}
