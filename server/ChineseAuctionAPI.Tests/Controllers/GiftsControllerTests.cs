using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ChineseAuctionAPI.Controllers;
using ChineseAuctionAPI.Services;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Repositories;

namespace ChineseAuctionAPI.Tests.Controllers
{
    public class GiftsControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var mockSvc = new Mock<IGiftService>();
            mockSvc.Setup(s => s.GetAllGiftsAsync()).ReturnsAsync((IEnumerable<Gift?>)new List<Gift?> { (Gift?)new Gift { Name = "G1" } });
            var ctrl = new GiftController(mockSvc.Object, Mock.Of<IUserRepo>(), Mock.Of<ILogger<GiftController>>());

            var res = await ctrl.Get();
            var ok = Assert.IsType<OkObjectResult>(res);
            ((IEnumerable<Gift>)ok.Value).Should().ContainSingle(g => g.Name == "G1");
        }

        [Fact]
        public async Task GetById_ReturnsOk()
        {
            var mock = new Mock<IGiftService>();
            mock.Setup(s => s.GetGiftByIdAsync(It.IsAny<int>())).ReturnsAsync((Gift?)new Gift { IdGift = 1 });
            var ctrl = new GiftController(mock.Object, Mock.Of<IUserRepo>(), Mock.Of<ILogger<GiftController>>());
            var res = await ctrl.Get(1);
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async Task Delete_NotFoundHandled()
        {
            var mock = new Mock<IGiftService>();
            mock.Setup(s => s.DeleteGiftAsync(It.IsAny<int>())).ReturnsAsync(false);
            var ctrl = new GiftController(mock.Object, Mock.Of<IUserRepo>(), Mock.Of<ILogger<GiftController>>());
            var res = await ctrl.Delete(1);
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public async Task DrawWinner_BadRequestWhenNull()
        {
            var mock = new Mock<IGiftService>();
            mock.Setup(s => s.DrawWinnerForGiftAsync(It.IsAny<int>())).ReturnsAsync((Winner?)null);
            var mockUserRepo = new Mock<IUserRepo>();
            var ctrl = new GiftController(mock.Object, mockUserRepo.Object, Mock.Of<ILogger<GiftController>>());
            var res = await ctrl.DrawWinner(1);
            Assert.IsType<BadRequestObjectResult>(res);
        }
    }
}
