using System.ComponentModel.DataAnnotations;

namespace ChineseAuctionAPI.DTOs
{
    public class GiftDTO
    {
        public int IdGift { get; set; } // חובה לעדכון!

        [Required]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, Range(1, 500)]
        public int Amount { get; set; } // שיניתי מ-Quantity ל-Amount שיתאים לאנגולר

        public string? Image { get; set; }

        [Required]
        public int IdDonor { get; set; }
    }
    public class GiftNewDTO
    {
        public int IdGift { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumOfBuyers { get; set; }
    }
    public class GiftUpdateDto
    {
        public int UserId { get; set; }
        public int GiftId { get; set; }
        public int Amount { get; set; }
    }
}
