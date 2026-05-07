using System.Data;
using ChineseAuctionAPI.Data;
using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace ChineseAuctionAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _config;

        public UserService(IUserRepo userRepository, IConfiguration config, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
            _config = config;
        }

        public static UserDTO MapToUserDto(User u)
        {
            return new UserDTO
            {
                Identity = u.Identity,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("מתחיל שליפת כל המשתמשים.");
                var users = await _userRepository.GetAllAsync();
                return users.Select(u => MapToUserDto(u)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשליפת רשימת המשתמשים.");
                throw new Exception("שגיאה בשליפת רשימת המשתמשים", ex);
            }
        }

        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("שולף משתמש לפי מזהה: {Id}", id);
                var user = await _userRepository.GetByIdAsync(id);
                return user != null ? MapToUserDto(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשליפת משתמש עם מזהה {Id}", id);
                throw new Exception($"שגיאה בשליפת משתמש עם מזהה {id}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("מוחק משתמש מזהה: {Id}", id);
                return await _userRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה במחיקת משתמש מזהה {Id}", id);
                throw new Exception($"Error deleting user with ID {id}", ex);
            }
        }
        public async Task<DtoUserOrder?> GetUserWithOrdersAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithOrdersAsync(userId);
                if (user == null) return null;

                return new DtoUserOrder
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Orders = user.Orders?.Select(o => new OrderDTO
                    {
                        Status = o.Status,
                        OrderDate = o.OrderDate,
                        IdUser = o.IdUser,
                        // מיפוי החבילות לפי ה-DTO החדש שלך:
                        OrdersPackages = o.OrdersPackage?.Select(op => new OrdersPackageDTO
                        {
                            IdPackage = op.IdPackage, // אות גדולה כמו ב-DTO שלך
                            Quantity = op.Quantity     // ודאי שבמודל המקורי זה Amount או Quantity
                        }).ToList() ?? new List<OrdersPackageDTO>(),

                        OrdersGifts = o.OrdersGift?.Select(ord => new OrdersGiftDTO
                        {
                            Name = ord.Gift?.Name ?? "",
                            Amount = ord.Amount,
                            Description = ord.Gift?.Description ?? ""
                        }).ToList() ?? new List<OrdersGiftDTO>()
                    }).ToList() ?? new List<OrderDTO>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
                return null;
            }
        }
        //public async Task<DtoUserOrder?> GetUserWithOrdersAsync(int userId)
        //{
        //    try
        //    {
        //        _logger.LogInformation("שולף משתמש {UserId} עם הזמנות.", userId);
        //        var user = await _userRepository.GetUserWithOrdersAsync(userId);

        //        if (user == null) return null;

        //        return new DtoUserOrder
        //        {
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            Orders = user.Orders?.Select(o => new OrderDTO
        //            {
        //                Status = o.Status,
        //                OrderDate = o.OrderDate,
        //                IdUser = o.IdUser,
        //                OrdersGifts = o.OrdersGift.Select(ord => new OrdersGiftDTO
        //                {
        //                    Name = ord.Gift.Name,
        //                    Amount = ord.Amount,
        //                    Price = ord.Gift.Price,
        //                    Description = ord.Gift.Description,
        //                    Image = ord.Gift.Image
        //                }).ToList()
        //            }).ToList() ?? new List<OrderDTO>()
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "שגיאה בשליפת משתמש והזמנות עבור מזהה {UserId}", userId);
        //        return null;
        //    }
        //}


        public async Task<string?> RegisterAsync(DtoLogin dto)
        {
            try
            {
                _logger.LogInformation("ניסיון רישום משתמש חדש: {Email}", dto?.Email);

                if (dto == null) throw new ArgumentNullException(nameof(dto));
                if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                    return null;

                var email = dto.Email.Trim();

                if (await _userRepository.ExistEmailAsync(email))
                {
                    _logger.LogWarning("ניסיון רישום נכשל: אימייל {Email} כבר קיים.", email);
                    return null;
                }

                var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var newUser = new User
                {
                    Identity = dto.Identity,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PasswordHash = hashed,
                    Email = email,
                    PhoneNumber = dto.PhoneNumber,
                    City = dto.City,
                    Address = dto.Address,
                    Orders = new List<Order>(),
                    Cards = new List<Card>()
                };

                var created = await _userRepository.AddAsync(newUser);
                var token = GenerateJwtToken(created);

                _logger.LogInformation("משתמש נרשם בהצלחה: {UserId}", created.IdUser);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה ברישום משתמש חדש.");
                throw new Exception("שגיאה בהרשמת משתמש חדש", ex);
            }
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation("ניסיון התחברות עבור אימייל: {Email}", email);

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return null;

                var u = await _userRepository.GetByEmailAsync(email.Trim());
                if (u == null)
                {
                    _logger.LogWarning("משתמש לא נמצא: {Email}", email);
                    return null;
                }

                if (!BCrypt.Net.BCrypt.Verify(password, u.PasswordHash))
                {
                    _logger.LogWarning("סיסמה שגויה עבור: {Email}", email);
                    return null;
                }

                return GenerateJwtToken(u);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בתהליך ההתחברות.");
                throw;
            }
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var jwtSection = _config.GetSection("Jwt");
                var keyStr = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing from configuration");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.IsManager.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSection["Issuer"],
                    audience: jwtSection["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה ביצירת JWT Token.");
                throw;
            }
        }
    }
}