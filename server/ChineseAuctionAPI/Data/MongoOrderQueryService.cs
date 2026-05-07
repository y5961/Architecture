using ChineseAuctionAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChineseAuctionAPI.Data
{
    public class MongoOrderQueryService
    {
        private readonly IMongoCollection<MongoOrderDocument> _orders;

        public MongoOrderQueryService(IMongoDatabase database)
        {
            _orders = database.GetCollection<MongoOrderDocument>("orders");
        }

        public async Task<List<MongoOrderDocument>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _orders.Find(o => o.Status == status).ToListAsync();
        }

        public async Task<List<MongoOrderDocument>> GetOrdersByUserAndStatusAsync(int userId, OrderStatus status)
        {
            var filter = Builders<MongoOrderDocument>.Filter.And(
                Builders<MongoOrderDocument>.Filter.Eq(o => o.IdUser, userId),
                Builders<MongoOrderDocument>.Filter.Eq(o => o.Status, status));

            return await _orders.Find(filter).ToListAsync();
        }

        public async Task<List<MongoOrderDocument>> GetOrdersWithMinimumGiftLinesAsync(int minimumGiftCount)
        {
            var filter = Builders<MongoOrderDocument>.Filter.Where(o => o.OrdersGift.Count > minimumGiftCount);
            return await _orders.Find(filter).ToListAsync();
        }

        public async Task<List<MongoOrderDocument>> GetRecentOrdersSinceAsync(DateTime since)
        {
            var filter = Builders<MongoOrderDocument>.Filter.Gte(o => o.OrderDate, since);
            return await _orders.Find(filter).SortByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<List<TopPackageSalesDto>> GetTopPackageSalesAsync(int topN = 5)
        {
            return await _orders.Aggregate()
                .Unwind<MongoOrderDocument, BsonDocument>(o => o.OrdersPackage)
                .Group(new BsonDocument
                {
                    { "_id", "$OrdersPackage.IdPackage" },
                    { "TotalQuantity", new BsonDocument("$sum", "$OrdersPackage.Quantity") }
                })
                .Sort(new BsonDocument("TotalQuantity", -1))
                .Limit(topN)
                .Project<TopPackageSalesDto>(new BsonDocument
                {
                    { "_id", 0 },
                    { "PackageId", "$_id" },
                    { "TotalQuantity", 1 }
                })
                .ToListAsync();
        }
    }

    public class TopPackageSalesDto
    {
        public int PackageId { get; set; }
        public int TotalQuantity { get; set; }
    }
}
