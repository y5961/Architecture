namespace ChineseAuctionAPI.Repositories
{
    using ChineseAuctionAPI.Data;
    using ChineseAuctionAPI.Models;
    using Microsoft.EntityFrameworkCore;

    public class GiftCategoryRepo : IGiftCategoryRepo
    {
        private readonly SaleContextDB _context;



        public GiftCategoryRepo(SaleContextDB context)
        {
            _context = context;

        }

        public async Task<List<GiftCategory>> GetAllAsync()
        {
            return await _context.GiftCategories.ToListAsync();
        }

        public async Task<GiftCategory?> GetByIdAsync(int id)
        {
            return await _context.GiftCategories.FindAsync(id);
        }

        public async Task<GiftCategory> CreateAsync(GiftCategory category)
        {
            _context.GiftCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(GiftCategory category)
        {
            _context.GiftCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(GiftCategory category)
        {
            _context.GiftCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

}
