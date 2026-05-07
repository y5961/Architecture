using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.DTOs
{
  

    // 2. הגדרת ה-DTO עבור המתנה בתוך ההזמנה
    public class OrdersGiftDTO
    {
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Range(1, 1000)]
        public int Amount { get; set; } = 1;
        [Required, Range(1, int.MaxValue)]
        public int Price { get; set; }
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public string? Image { get; set; }
    }

    // 3. אובייקט ההזמנה המלא שמחבר את הכל
    public class OrderDTO
    {
        [Required]
        public int IdOrder { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int TotalAmount { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int TotalPrice { get; set; }
        [Required]
        public int IdUser { get; set; }
        [Range(1, 1000)]
        public int Amount { get; set; } = 1;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        // הרשימות שמאפשרות לנתונים לעבור לאנגולר
        public List<OrdersPackageDTO> OrdersPackages { get; set; } = new();
        public List<OrdersGiftDTO> OrdersGifts { get; set; } = new();
    }
}