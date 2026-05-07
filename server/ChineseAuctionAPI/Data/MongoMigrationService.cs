using ChineseAuctionAPI.Models;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace ChineseAuctionAPI.Data
{
    public class MongoMigrationService
    {
        private readonly SaleContextDB _relationalContext;
        private readonly IMongoCollection<MongoOrderDocument> _orderCollection;

        public MongoMigrationService(SaleContextDB relationalContext, IMongoDatabase mongoDatabase)
        {
            _relationalContext = relationalContext;
            _orderCollection = mongoDatabase.GetCollection<MongoOrderDocument>("orders");
        }

        public async Task MigrateOrdersAsync(bool dropExisting = true)
        {
            if (dropExisting)
            {
                await _orderCollection.DeleteManyAsync(Builders<MongoOrderDocument>.Filter.Empty);
            }

            var orders = await _relationalContext.OrdersOrders
                .Include(o => o.OrdersGift)
                .Include(o => o.OrdersPackage)
                .ToListAsync();

            var docs = orders.Select(o => new MongoOrderDocument
            {
                IdOrder = o.IdOrder,
                IdUser = o.IdUser,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Price = o.Price,
                OrdersGift = o.OrdersGift.Select(g => new MongoOrderGift
                {
                    IdOrdersGift = g.IdOrdersGift,
                    IdGift = g.IdGift,
                    Amount = g.Amount
                }).ToList(),
                OrdersPackage = o.OrdersPackage.Select(p => new MongoOrderPackage
                {
                    IdPackageOrder = p.IdPackageOrder,
                    IdPackage = p.IdPackage,
                    Quantity = p.Quantity,
                    PriceAtPurchase = p.PriceAtPurchase
                }).ToList()
            }).ToList();

            if (docs.Any())
            {
                await _orderCollection.InsertManyAsync(docs);
            }
        }
    }
}
