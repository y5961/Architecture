using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ChineseAuctionAPI.Services;
using ChineseAuctionAPI.Repositories;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.DTOs;

namespace ChineseAuctionAPI.Tests.Services
{
    public class GiftServiceTests
    {
        [Fact]
        public async Task GetAllMapsToDto()
        {
            var mockRepo = new Mock<IGiftRepo>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync((IEnumerable<Gift?>)new List<Gift?> { (Gift?)new Gift { IdGift = 1, Name = "G1", Price = 10 } });
            var svc = new GiftService(mockRepo.Object, Mock.Of<ILogger<GiftService>>(), Mock.Of<IConfiguration>(), Mock.Of<IEmailService1>(), Mock.Of<IUserRepo>());

            var res = (await svc.GetAllGiftsAsync()).ToList();
            res.Should().HaveCount(1);
            res.First().Name.Should().Be("G1");
        }

        [Fact]
        public async Task AddAndDelete_UseRepository()
        {
            var mockRepo = new Mock<IGiftRepo>();
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Gift>())).ReturnsAsync((Gift?)new Gift { IdGift = 5 });
            mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);
            var svc = new GiftService(mockRepo.Object, Mock.Of<ILogger<GiftService>>(), Mock.Of<IConfiguration>(), Mock.Of<IEmailService1>(), Mock.Of<IUserRepo>());

            var dto = new GiftDTO { Name = "X", Description = "D", Price = 1, Amount = 1 };
            var created = await svc.CreateGiftAsync(dto);
            created.IdGift.Should().BeGreaterThan(0);

            var deleted = await svc.DeleteGiftAsync(created.IdGift);
            deleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetById_ReturnsNullWhenMissing()
        {
            var mock = new Mock<IGiftRepo>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Gift?)null);
            var svc = new GiftService(mock.Object, Mock.Of<ILogger<GiftService>>(), Mock.Of<IConfiguration>(), Mock.Of<IEmailService1>(), Mock.Of<IUserRepo>());
            var res = await svc.GetGiftByIdAsync(999);
            res.Should().BeNull();
        }

        [Fact]
        public async Task SortByPrice_Delegates()
        {
            var mock = new Mock<IGiftRepo>();
            mock.Setup(r => r.SortByPrice()).ReturnsAsync((IEnumerable<Gift?>)new List<Gift?>());
            var svc = new GiftService(mock.Object, Mock.Of<ILogger<GiftService>>(), Mock.Of<IConfiguration>(), Mock.Of<IEmailService1>(), Mock.Of<IUserRepo>());
            var res = await svc.SortByPrice();
            res.Should().NotBeNull();
        }

        [Fact]
        public async Task Add_EmailAttempt_NoThrow()
        {
            var mock = new Mock<IGiftRepo>();
            mock.Setup(r => r.AddAsync(It.IsAny<Gift>())).ReturnsAsync((Gift?)new Gift { IdGift = 1 });
            var svc = new GiftService(mock.Object, Mock.Of<ILogger<GiftService>>(), Mock.Of<IConfiguration>(), Mock.Of<IEmailService1>(), Mock.Of<IUserRepo>());
            var g = await svc.CreateGiftAsync(new GiftDTO { Name = "N" });
            g.IdGift.Should().Be(1);
        }

        [Fact]
        public async Task GetParticipants_EmptyHandled()
        {
            var mock = new Mock<IGiftRepo>();
            mock.Setup(r => r.GetGiftWithOrdersAndUsersAsync(It.IsAny<int>())).ReturnsAsync((Gift?)null);
            var svc = new GiftService(mock.Object, Mock.Of<ILogger<GiftService>>(), Mock.Of<IConfiguration>(), Mock.Of<IEmailService1>(), Mock.Of<IUserRepo>());
            var res = await svc.GetParticipantsDetailsAsync(1);
            res.Should().BeEmpty();
        }
    }
}
