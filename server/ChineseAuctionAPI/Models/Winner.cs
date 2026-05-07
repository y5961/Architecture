using System.ComponentModel.DataAnnotations;
using ChineseAuctionAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChineseAuctionAPI.Models
{
    public class Winner
    {
        [Key]

        public int IdWinner { get; set; }
        [Required]
        public int IdUser { get; set; }
        [ForeignKey("IdUser")]

        public virtual User User { get; set; }
        [Required]
        public int IdGift { get; set; }
        [ForeignKey("IdGift")]
        public virtual Gift Gift { get; set; }

    }
}

