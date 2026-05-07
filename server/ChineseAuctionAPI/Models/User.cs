using System.ComponentModel.DataAnnotations;
using ChineseAuctionAPI.Models;

namespace ChineseAuctionAPI.Models
{
    public enum Role
    {
        User,
        Manager,
    }
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        [MinLength(6, ErrorMessage = "הסיסמה חייבת להכיל לפחות 6 תווים")]
        public string PasswordHash { get; set; }
        [Required(ErrorMessage = "תעודת זהות היא שדה חובה")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "תעודת זהות חייבת להכיל 9 ספרות")]
        public string Identity { get; set; }
        [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "כתובת אימייל היא שדה חובה")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "מספר טלפון הוא שדה חובה")]
        [Phone(ErrorMessage = "מספר טלפון לא תקין")]
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        [Required]
        public Role IsManager { get; set; } = Role.User;
        public ICollection<Order> Orders { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}