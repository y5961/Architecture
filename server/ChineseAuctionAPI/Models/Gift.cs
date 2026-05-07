using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChineseAuctionAPI.Models
{
    public class Gift
    {
        [Key]
        public int IdGift { get; set; }

        [Required(ErrorMessage = "שם המתנה הוא שדה חובה")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "שם המתנה חייב להיות בין 2 ל-100 תווים")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "תיאור המתנה לא יכול לעלות על 500 תווים")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "קטגוריה היא שדה חובה")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual GiftCategory Category { get; set; }

        [Range(1, 1000, ErrorMessage = "הכמות חייבת להיות לפחות 1")]
        public int Amount { get; set; } = 1;

        public string? Image { get; set; }

        [Required(ErrorMessage = "מזהה תורם הוא שדה חובה")]
        public int IdDonor { get; set; }

        [ForeignKey("IdDonor")]
        public  Donor Donor { get; set; }

        public bool IsDrawn { get; set; } = false;

        public int? IdUser { get; set; }

        [ForeignKey("IdUser")]
        public virtual User? user { get; set; }

        public ICollection<OrdersGift> OrdersGifts { get; set; }
       

    }
}