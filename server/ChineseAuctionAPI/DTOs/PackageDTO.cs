using System.ComponentModel.DataAnnotations;

namespace ChineseAuctionAPI.DTOs
{
    public class PackageDTO
    {
        [Required]
        public int IdPackage { get; set; }
        public int AmountRegular { get; set; }
        public int? AmountPremium { get; set; }
        [Required,Range(0,int.MaxValue)]
        public int Price { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class PackageCreateDTO
    {
        [Required]
        public int AmountRegular { get; set; }
        public int? AmountPremium { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Price { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class OrdersPackageDTO
    {
        public int IdPackage { get; set; } // שימי לב לאותיות קטנות כדי שיתאים ל-Angular שלך
        public int Quantity { get; set; }
    }
}