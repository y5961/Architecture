using System.Reflection;
using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Services
{
    public interface IGiftService
    {
        Task<bool> UpdateGiftAsync(int id, GiftDTO dto);

        Task<Gift> CreateGiftAsync(GiftDTO dto);
        Task<bool> DeleteGiftAsync(int id);
        Task<IEnumerable<Gift>> GetAllGiftsAsync();
        Task<Gift?> GetGiftByIdAsync(int id);
        Task<Winner?> DrawWinnerForGiftAsync(int giftId);
        Task<IEnumerable<Gift?>> GetByNameGift(string word);
        Task<IEnumerable<Gift?>> GetByNameDonor(string donor);
        Task<IEnumerable<GiftNewDTO?>> GetByNumOfBuyers(int buyers);
        //Task<IEnumerable<Gift?>> SortByPrice();
        Task<IEnumerable<Gift?>> SortByAmountPeople();
        Task<IEnumerable<ParticipantDetailsDTO>> GetParticipantsDetailsAsync(int giftId);
        Task<IEnumerable<Winner>> GetAllWinnersAsync();

    }

}