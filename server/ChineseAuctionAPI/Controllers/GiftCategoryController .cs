using global::ChineseAuctionAPI.DTOs;
using global::ChineseAuctionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChineseAuctionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftCategoryController : ControllerBase
    {
        private readonly IGiftCategoryService _service;
        private readonly ILogger<GiftCategoryController> _logger;

        public GiftCategoryController(IGiftCategoryService service, ILogger<GiftCategoryController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all gift categories.");
                var categories = await _service.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all gift categories.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve gift category with ID: {Id}", id);
                var result = await _service.GetByIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning("Gift category with ID: {Id} was not found.", id);
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving gift category with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> Create(CreateGiftCategoryDTO dto)
        {
            try
            {
                _logger.LogInformation("Attempting to create a new gift category.");
                var result = await _service.CreateAsync(dto);
                _logger.LogInformation("Gift category created successfully with ID: {Id}", result.Id);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a gift category.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> Update(int id, UpdateGiftCategoryDTO dto)
        {
            try
            {
                _logger.LogInformation("Attempting to update gift category with ID: {Id}", id);
                var success = await _service.UpdateAsync(id, dto);

                if (!success)
                {
                    _logger.LogWarning("Update failed. Gift category with ID: {Id} not found.", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating gift category with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete gift category with ID: {Id}", id);
                var success = await _service.DeleteAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Delete failed. Gift category with ID: {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Gift category with ID: {Id} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting gift category with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}