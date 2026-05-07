using System.ComponentModel.DataAnnotations;

namespace ChineseAuctionAPI.Models
{
    public class Package
    {
        [Key]
        public int IdPackage { get; set; }
        [Range(0, 1000)]
        public int AmountRegular { get; set; }
        [Range(0, 1000)]
        public int? AmountPremium { get; set; }
        public int Price { get; set; }
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public ICollection<Gift> Gifts { get; set; }
        public ICollection<OrdersPackage> OrdersPackage { get; set; }
    }
}
