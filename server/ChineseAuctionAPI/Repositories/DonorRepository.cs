using System.Linq;
using ChineseAuctionAPI.Data;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Repositories;
using Microsoft.EntityFrameworkCore;

public class DonorRepository : IDonorRepository
{
    private readonly SaleContextDB _context;
   


    public DonorRepository(SaleContextDB context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Donor>> GetAllAsync()
    {
        return await _context.Donors.ToListAsync();
    }

    public async Task<Donor?> GetByIdAsync(int id)
    {
        return await _context.Donors.FindAsync(id);
    }

    public async Task<int> AddAsync(Donor donor)
    {
        _context.Donors.Add(donor);
        await _context.SaveChangesAsync();
        return donor.IdDonor;
    }

    public async Task UpdateAsync(Donor donor)
    {
        _context.Entry(donor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var donor = await _context.Donors.FindAsync(id);
        if (donor != null)
        {
            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Gift>>? GetGiftsAsync(int DonorId)
    {
        return await _context.Gifts.Where(go=>go.IdDonor== DonorId).Include(c=>c.Category).ToListAsync();
    }

    public async Task<Donor> SortByGift(string donor)
    {
        var gift=await _context.Gifts.Include(g=>g.Donor).FirstOrDefaultAsync(g=>g.Name==donor);

        return gift?.Donor;


    }

    public async Task<IEnumerable<Donor?>> SortByEmail(string email)
    {
        return await _context.Donors
            .Where(d => d.Email.Contains(email)).ToListAsync();
    }

    public async Task<IEnumerable<Donor?>> SortByName(string name)
    {
        return await _context.Donors
         .Where(d => d.FirstName.Contains(name) || d.LastName.Contains(name))
         .ToListAsync();
    }
}