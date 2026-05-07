using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;

public interface IDonorService
{
    Task<IEnumerable<DonorDTO>> GetAllDonorsAsync();
    Task<DonorDTO> GetDonorByIdAsync(int id);
    Task<int> CreateDonorAsync(DonorCreateDTO donorDto);
    Task UpdateDonorAsync(int id, DonorCreateDTO donorDto);
    Task DeleteDonorAsync(int id);
    Task<IEnumerable<GiftDTO>>? GetGiftsAsync(int donorId);
    Task<Donor?> SortByGift(string donor);
    Task<IEnumerable<Donor?>> SortByEmail(string email);
    Task<IEnumerable<Donor?>> SortByName(string name);

}