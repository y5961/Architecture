using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChineseAuctionAPI.Data;
using ChineseAuctionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChineseAuctionAPI.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly SaleContextDB _context;


        public UserRepo(SaleContextDB context)
        {
            _context = context;

        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.IdUser == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.IdUser == id);
        }

        public async Task<User?> GetUserWithOrdersAsync(int userId)
        {
            return await _context.Users
        .Include(u => u.Orders)
            .ThenInclude(o => o.OrdersPackage) // <--- הוספה קריטית: טעינת החבילות
        .Include(u => u.Orders)
            .ThenInclude(o => o.OrdersGift)    // טעינת המתנות
                .ThenInclude(og => og.Gift)
        .FirstOrDefaultAsync(u => u.IdUser == userId);
        }

        public async Task<User?> GetUserWithCardsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Cards)
                .FirstOrDefaultAsync(u => u.IdUser == userId);
        }
    }
}