
using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChineseAuctionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DtoLogin dto)
        {
            try
            {
                _logger.LogInformation("Attempting to register user with email: {Email}", dto.Email);
                var resp = await _userService.RegisterAsync(dto);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for email: {Email}", dto.Email);
                return StatusCode(500, "Internal server error during registration");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DtologinRequest dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", dto.Email);
                var resp = await _userService.LoginAsync(dto.Email, dto.Password);

                if (resp == null)
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", dto.Email);
                    return Unauthorized("Invalid credentials");
                }

                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", dto.Email);
                return StatusCode(500, "Internal server error during login");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete user with ID: {Id}", id);
                var result = await _userService.DeleteAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Delete failed. User with ID: {Id} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("User with ID: {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetUserWithOrders(int id)
        {
            try
            {
                _logger.LogInformation("Fetching user with orders for ID: {Id}", id);
                var userWithOrders = await _userService.GetUserWithOrdersAsync(id);
                if (userWithOrders == null)
                {
                    _logger.LogWarning("User with ID: {Id} not found for orders request", id);
                    return NotFound();
                }
                return Ok(userWithOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for user ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching user details for ID: {Id}", id);
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID: {Id} not found", id);
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
