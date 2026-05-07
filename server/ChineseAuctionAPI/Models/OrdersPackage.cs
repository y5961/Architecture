using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChineseAuctionAPI.Models
{
    public class OrdersPackage
    {
        [Key]
        public int IdPackageOrder { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [Required]
        public int IdPackage { get; set; }
        [ForeignKey("IdPackage")]
        public virtual Package Package { get; set; }
        [Range(1,100)]
        public int Quantity { get; set; } = 1;
        [Required]
        [Range(0,int.MaxValue)]
        public int PriceAtPurchase { get; set; }
    }
}

