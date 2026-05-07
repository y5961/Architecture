using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ChineseAuctionAPI.Services;
using ChineseAuctionAPI.Repositories;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task GetDraftOrderByUser_ReturnsDto_WhenExists()
        {
            var mockRepo = new Mock<IOrderRepo>();
            mockRepo.Setup(r => r.GetDraftOrderByUserAsync(It.IsAny<int>())).ReturnsAsync((Order?)new Order { IdOrder = 9, IdUser = 1 });
            var svc = new OrderService(mockRepo.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<OrderService>>());

            var dto = await svc.GetDraftOrderByUserAsync(1);
            dto.Should().NotBeNull();
            dto.IdOrder.Should().Be(9);
        }

            [Fact]
            public async Task GetIncomeReport_Delegates()
            {
                var mock = new Mock<IOrderRepo>();
                mock.Setup(r => r.GetIncomeReportAsync()).ReturnsAsync((IncomeReportDTO?)new IncomeReportDTO());
                var svc = new OrderService(mock.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<OrderService>>());
                var r = await svc.GetIncomeReportAsync();
                r.Should().NotBeNull();
            }

            [Fact]
            public async Task UpdatePackage_ReturnsTrue()
            {
                var mock = new Mock<IOrderRepo>();
                mock.Setup(r => r.AddOrUpdatePackageInOrderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);
                var svc = new OrderService(mock.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<OrderService>>());
                var ok = await svc.UpdatePackageQuantityAsync(1,1,1);
                ok.Should().BeTrue();
            }

            [Fact]
            public async Task GetById_NullHandled()
            {
                var mock = new Mock<IOrderRepo>();
                mock.Setup(r => r.GetByIdWithGiftsAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);
                var svc = new OrderService(mock.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<OrderService>>());
                var res = await svc.GetByIdWithGiftsAsync(1);
                res.Should().BeNull();
            }
    }
}
