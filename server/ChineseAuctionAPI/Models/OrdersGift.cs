using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChineseAuctionAPI.Models
{
    public class OrdersGift
    {
        [Key]

        public int IdOrdersGift { get; set; }
        public int IdGift { get; set; }
        [ForeignKey("IdGift")]
        public Gift Gift { get; set; } = null!;
        public int IdOrder { get; set; }
        [ForeignKey("IdOrder")]
        public Order Order { get; set; } = null!;
        public int Amount { get; set; }
    }
}
