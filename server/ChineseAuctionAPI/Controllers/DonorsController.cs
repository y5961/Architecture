using ChineseAuctionAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager")]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _service;
    private readonly ILogger<DonorsController> _logger;

    public DonorsController(IDonorService service, ILogger<DonorsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonorDTO>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Fetching all donors");
            var donors = await _service.GetAllDonorsAsync();
            return Ok(donors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all donors");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DonorDTO>> Get(int id)
    {
        try
        {
            _logger.LogInformation("Fetching donor with ID: {Id}", id);
            var donor = await _service.GetDonorByIdAsync(id);
            if (donor == null)
            {
                _logger.LogWarning("Donor with ID: {Id} not found", id);
                return NotFound();
            }
            return Ok(donor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching donor with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(DonorCreateDTO dto)
    {
        try
        {
            _logger.LogInformation("Creating a new donor");
            var id = await _service.CreateDonorAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a donor");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DonorCreateDTO dto)
    {
        try
        {
            _logger.LogInformation("Updating donor with ID: {Id}", id);
            await _service.UpdateDonorAsync(id, dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating donor with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting donor with ID: {Id}", id);
            await _service.DeleteDonorAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting donor with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("sort/name/{name}")]

    public async Task<IActionResult> SortByName(string name)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve donor with name: {name}", name);
            var name1 = await _service.SortByName(name);

            if (name1 == null)
            {
                _logger.LogWarning("donor with word: {name} not found.", name);

            }
            return Ok(name1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching donor with name: {name}", name);
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpGet("sort/email/{email}")]

    public async Task<IActionResult> SortByEmail(string email)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve donor with email: {email}", email);
            var email1 = await _service.SortByEmail(email);

            if (email1 == null)
            {
                _logger.LogWarning("donor with email: {email} not found.", email);

            }
            return Ok(email1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching donor with email: {email}", email);
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpGet("sort/gift/{gift}")]

    public async Task<IActionResult> SortByGift(string gift)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve donor with gift: {gift}", gift);
            var gift1 = await _service.SortByGift(gift);

            if (gift1 == null)
            {
                _logger.LogWarning("donor with gift: {gift} not found.", gift);

            }
            return Ok(gift1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching donor with gift: {gift}", gift);
            return StatusCode(500, "Internal server error");
        }
    }
}