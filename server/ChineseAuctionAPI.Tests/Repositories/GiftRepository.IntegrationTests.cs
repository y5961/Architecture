using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ChineseAuctionAPI.Tests.TestHelpers;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Repositories;

public class GiftRepositoryIntegrationTests
{
    [Fact]
    public async Task GetAll_GetById_Add_Update_Delete()
    {
        using var ctx = DbTestHelper.CreateInMemoryContext("gift_repo_test");
        await DbTestHelper.SeedDatabaseAsync(ctx);

        var repo = new GiftRepo(ctx);

        var all = (await repo.GetAllAsync()).ToList();
        Assert.NotNull(all);

        var seeded = all.FirstOrDefault();
        if (seeded != null)
        {
            var byId = await repo.GetByIdAsync(seeded.IdGift);
            Assert.NotNull(byId);
        }

        // prepare related entities
        var category = ctx.GiftCategories.First();
        var donor = ctx.Donors.First();

        var newGift = new Gift
        {
            Name = "GTest",
            Description = "d",
            CategoryId = category.IdGiftCategory,
            IdDonor = donor.IdDonor,
            Price = 100,
            Amount = 1
        };

        var added = await repo.AddAsync(newGift);
        Assert.NotNull(added);
        Assert.Equal("GTest", added.Name);

        added.Price = 200;
        var ok = await repo.UpdateAsync(added);
        Assert.True(ok);

        var deletedOk = await repo.DeleteAsync(added.IdGift);
        Assert.True(deletedOk);
    }

    [Fact]
    public async Task Repo_Gift_CRUD_Smoke()
    {
        using var ctx = DbTestHelper.CreateInMemoryContext("r4-gift");
        await DbTestHelper.SeedDatabaseAsync(ctx);
        var repo = new GiftRepo(ctx);
        var gift = new Gift { Name = "Gx", Price = 1, Amount = 1, Category = ctx.GiftCategories.First(), Donor = ctx.Donors.First() };
        var added = await repo.AddAsync(gift);
        Assert.NotNull(added);
        var byId = await repo.GetByIdAsync(added.IdGift);
        Assert.NotNull(byId);
        await repo.DeleteAsync(added.IdGift);
    }

    [Fact]
    public async Task Repo_Gift_GetByName_Returns()
    {
        using var ctx = DbTestHelper.CreateInMemoryContext("r10-gift");
        await DbTestHelper.SeedDatabaseAsync(ctx);
        var repo = new GiftRepo(ctx);
        var res = await repo.GetByNameGift("Gift");
        Assert.NotNull(res);
    }
}
