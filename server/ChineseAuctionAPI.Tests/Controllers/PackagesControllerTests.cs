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
    public class PackagesControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var mockSvc = new Mock<IPackageService>();
            mockSvc.Setup(s => s.GetAllPackagesAsync()).ReturnsAsync((IEnumerable<PackageDTO?>)new List<PackageDTO?> { (PackageDTO?)new PackageDTO { Name = "P" } });
            var ctrl = new PackagesController(mockSvc.Object, Mock.Of<ILogger<PackagesController>>());

            var res = await ctrl.GetAll();
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            ((IEnumerable<PackageDTO>)ok.Value).Should().ContainSingle(p => p.Name == "P");
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mockSvc = new Mock<IPackageService>();
            mockSvc.Setup(s => s.CreatePackageAsync(It.IsAny<PackageCreateDTO>())).ReturnsAsync((int)5);
            var ctrl = new PackagesController(mockSvc.Object, Mock.Of<ILogger<PackagesController>>());

            var res = await ctrl.Create(new PackageCreateDTO { Name = "p" });
            var created = Assert.IsType<CreatedAtActionResult>(res.Result);
            ((int)created.Value).Should().Be(5);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var mockSvc = new Mock<IPackageService>();
            mockSvc.Setup(s => s.DeletePackageAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            var ctrl = new PackagesController(mockSvc.Object, Mock.Of<ILogger<PackagesController>>());

            var res = await ctrl.Delete(1);
            Assert.IsType<NoContentResult>(res);
        }
    }
}
