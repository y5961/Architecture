using System.Threading.Tasks;
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
    public class UserServiceTests
    {
        [Fact]
        public async Task RegisterAsync_ReturnsToken_WhenNewUser()
        {
            var mockRepo = new Mock<IUserRepo>();
            mockRepo.Setup(r => r.ExistEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User?)new User { IdUser = 7, Email = "u@x.com" });

            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>{ ["Jwt:Key"] = "01234567890123456789012345678901", ["Jwt:Issuer"] = "iss", ["Jwt:Audience"] = "aud" }).Build();
            var svc = new UserService(mockRepo.Object, config, Mock.Of<ILogger<UserService>>());

            var token = await svc.RegisterAsync(new DtoLogin { Email = "a@b.com", Password = "p" });
            token.Should().NotBeNull();
        }

            [Fact]
            public async Task Login_ReturnsNull_OnBadCreds()
            {
                var mock = new Mock<IUserRepo>();
                mock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
                var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>{ ["Jwt:Key"] = "01234567890123456789012345678901" }).Build();
                var svc = new UserService(mock.Object, config, Mock.Of<ILogger<UserService>>());
                var token = await svc.LoginAsync("a@b.com","p");
                token.Should().BeNull();
            }

            [Fact]
            public async Task GetAll_Maps()
            {
                var mock = new Mock<IUserRepo>();
                mock.Setup(r => r.GetAllAsync()).ReturnsAsync((IEnumerable<User?>)new List<User?>{ (User?)new User{ Email = "e@x.com"}});
                var svc = new UserService(mock.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<UserService>>());
                var res = await svc.GetAllAsync();
                res.Should().ContainSingle();
            }

            [Fact]
            public async Task Delete_ReturnsFalseIfMissing()
            {
                var mock = new Mock<IUserRepo>();
                mock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(false);
                var svc = new UserService(mock.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<UserService>>());
                var ok = await svc.DeleteAsync(1);
                ok.Should().BeFalse();
            }
    }
}
