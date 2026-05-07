using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChineseAuctionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _service;
        private readonly ILogger<PackagesController> _logger;

        public PackagesController(IPackageService service, ILogger<PackagesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageDTO>>> GetAll()
        {
            var packages = await _service.GetAllPackagesAsync();
            return Ok(packages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PackageDTO>> Get(int id)
        {
            var package = await _service.GetPackageByIdAsync(id);
            if (package == null) return NotFound();
            return Ok(package);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<int>> Create(PackageCreateDTO dto)
        {
            var id = await _service.CreatePackageAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Update(int id, PackageCreateDTO dto)
        {
            await _service.UpdatePackageAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeletePackageAsync(id);
            return NoContent();
        }
        // DTO פשוט עבור הקלט
        public class PackageUpdateDto
        {
            public int UserId { get; set; }
            public int PackageId { get; set; }
            public int Quantity { get; set; }
        }
    }
}