using System.ComponentModel.DataAnnotations;

namespace ChineseAuctionAPI.Models
{
    public class Donor
    {
        [Key]
        public int IdDonor { get; set; }
        [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        public string LastName { get; set; }
        [EmailAddress] 
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

    }
}
