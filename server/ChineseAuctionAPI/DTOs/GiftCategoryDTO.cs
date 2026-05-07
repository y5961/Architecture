using System.ComponentModel.DataAnnotations;

namespace ChineseAuctionAPI.DTOs
{
    public class GiftCategoryDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class CreateGiftCategoryDTO
    {
        [Required]
        public string Name { get; set; }
    }

    public class UpdateGiftCategoryDTO
    {
        [Required]
        public string Name { get; set; }
    }
}

