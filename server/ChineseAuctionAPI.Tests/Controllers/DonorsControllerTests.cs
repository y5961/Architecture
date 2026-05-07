using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ChineseAuctionAPI.Controllers;
using ChineseAuctionAPI.Services;
using ChineseAuctionAPI.DTOs;

namespace ChineseAuctionAPI.Tests.Controllers
{
    public class DonorsControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithDonors()
        {
            var mockSvc = new Mock<IDonorService>();
            mockSvc.Setup(s => s.GetAllDonorsAsync()).ReturnsAsync((IEnumerable<DonorDTO?>)new List<DonorDTO?> { (DonorDTO?)new DonorDTO { IdDonor = 1, Email = "a@b.com" } });
            var ctrl = new DonorsController(mockSvc.Object, Mock.Of<ILogger<DonorsController>>());

            var res = await ctrl.GetAll();
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            ((IEnumerable<DonorDTO>)ok.Value).Should().HaveCount(1);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenMissing()
        {
            var mockSvc = new Mock<IDonorService>();
            mockSvc.Setup(s => s.GetDonorByIdAsync(5)).ReturnsAsync((DonorDTO?)null);
            var ctrl = new DonorsController(mockSvc.Object, Mock.Of<ILogger<DonorsController>>());

            var res = await ctrl.Get(5);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAt()
        {
            var mockSvc = new Mock<IDonorService>();
            mockSvc.Setup(s => s.CreateDonorAsync(It.IsAny<DonorCreateDTO>())).ReturnsAsync(11);
            var ctrl = new DonorsController(mockSvc.Object, Mock.Of<ILogger<DonorsController>>());

            var res = await ctrl.Create(new DonorCreateDTO { FirstName = "A" });
            var created = Assert.IsType<CreatedAtActionResult>(res.Result);
            created.Value.Should().Be(11);
        }

        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            var mock = new Mock<IDonorService>();
            mock.Setup(s => s.UpdateDonorAsync(It.IsAny<int>(), It.IsAny<DonorCreateDTO>())).Returns(Task.CompletedTask);
            var ctrl = new DonorsController(mock.Object, Mock.Of<ILogger<DonorsController>>());
            var res = await ctrl.Update(1, new DonorCreateDTO { FirstName = "F" });
            Assert.IsType<NoContentResult>(res);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var mock = new Mock<IDonorService>();
            mock.Setup(s => s.DeleteDonorAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            var ctrl = new DonorsController(mock.Object, Mock.Of<ILogger<DonorsController>>());
            var res = await ctrl.Delete(1);
            Assert.IsType<NoContentResult>(res);
        }
    }
}
