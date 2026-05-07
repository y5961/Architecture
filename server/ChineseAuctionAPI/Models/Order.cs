using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChineseAuctionAPI.Models
{
    public enum OrderStatus
    {
        Draft,
        Completed,
    }

    public class Order
    {
        [Key]
        public int IdOrder { get; set; }
        [Required]
        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public User User { get; set; }
        public ICollection<OrdersPackage> OrdersPackage { get; set; }
        public DateTime OrderDate { get; set; }= DateTime.Now;
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Draft;
        public ICollection <OrdersGift> OrdersGift { get; set; }
        [Range(0,int.MaxValue,ErrorMessage ="מחיר לא יכול להיות שלילי")]
        public int Price { get; set; } = 1;

    }
}
