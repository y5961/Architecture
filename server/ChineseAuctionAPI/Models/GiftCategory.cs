using System.ComponentModel.DataAnnotations;

namespace ChineseAuctionAPI.Models
{
    public class GiftCategory
    {
        [Key]
        public int IdGiftCategory { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
