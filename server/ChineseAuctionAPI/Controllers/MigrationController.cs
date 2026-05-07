using ChineseAuctionAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace ChineseAuctionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly MongoMigrationService _migrationService;

        public MigrationController(MongoMigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> MigrateOrders()
        {
            try
            {
                await _migrationService.MigrateOrdersAsync(dropExisting: true);
                return Ok("Orders migrated to MongoDB successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Migration failed: {ex.Message}");
            }
        }
    }
}
