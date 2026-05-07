using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace ChineseAuctionAPI.DTOs
{
    public class UserDTO
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        [EmailAddress(ErrorMessage = "כתובת האימייל אינה תקינה")]
        public string? Email { get; set; }
        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "תעודת זהות חייבת להכיל 9 ספרות")]
        public string Identity { get; internal set; }

    }
    public class DtoLogin
    {
        [Required,MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "תעודת זהות חייבת להכיל 9 ספרות")]
        public string Identity { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "כתובת האימייל אינה תקינה")]
        public string? Email { get; set; }
        [Phone,Required]
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }

    public class DtoUserOrder
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public List<OrderDTO> Orders { get; set; } = new();
    }
    public class DtoLoginResponse
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public UserDTO User { get; set; }
    }
    public class DtologinRequest
    {
        [EmailAddress,Required]
        public string Email { get; set; }
        [Required,MinLength(6), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
