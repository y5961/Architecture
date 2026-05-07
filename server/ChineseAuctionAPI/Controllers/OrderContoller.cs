using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using static ChineseAuctionAPI.Controllers.PackagesController;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("income-report")]
    // [Authorize(Roles = "Manager")] // כדאי להוסיף אם יש לך הגדרת תפקידים
    public async Task<ActionResult<IncomeReportDTO>> GetIncomeReport()
    {
        try
        {
            var report = await _orderService.GetIncomeReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "שגיאה בהפקת הדוח: " + ex.Message);
        }
    }
    [HttpGet("draft/{userId}")]
    public async Task<ActionResult<OrderDTO>> GetDraftOrder(int userId)
    {
        try
        {
            var order = await _orderService.GetDraftOrderByUserAsync(userId);
            if (order == null) return NotFound("לא נמצאה טיוטת הזמנה");

            // כאן יעבור ה-ID 44 שמצאנו ב-SQL
            return Ok(order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders(int userId)
    {
        try
        {
            var orders = await _orderService.GetAllAsync(userId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // --- הפונקציות הקיימות שלך ---

    [HttpPost("update-package")]
    public async Task<ActionResult> UpdatePackageQuantity([FromBody] PackageUpdateDto dto)
    {
        try
        {
            var result = await _orderService.UpdatePackageQuantityAsync(dto.UserId, dto.PackageId, dto.Quantity);
            if (result) return Ok(true);
            return BadRequest("שגיאה בעדכון כמות החבילה");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("add-gift")]
    public async Task<ActionResult> AddOrUpdateGift([FromBody] GiftUpdateDto dto)
    {
        try
        {
            var result = await _orderService.AddOrUpdateGiftInOrderAsync(dto.UserId, dto.GiftId, dto.Amount);
            if (result) return Ok(true);
            return BadRequest("שגיאה בעדכון המתנה");
        }
        catch (InvalidOperationException ex) when (ex.Message == "INSUFFICIENT_TICKETS")
        {
            return BadRequest("INSUFFICIENT_TICKETS");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("complete/{orderId}")]
    public async Task<ActionResult> CompleteOrder(int orderId)
    {
        try
        {
            var result = await _orderService.CompleteOrder(orderId);
            if (result) return Ok("ההזמנה הושלמה בהצלחה");
            return BadRequest("לא ניתן היה להשלים את ההזמנה");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("draft/{userId}")]
    public async Task<ActionResult> DeleteDraft(int userId)
    {
        try
        {
            var result = await _orderService.DeleteDraftOrderByUserAsync(userId);
            if (result) return Ok(true);
            return NotFound("No draft found for user");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("draft/{userId}/gift/{giftId}")]
    public async Task<ActionResult> DeleteGiftFromDraft(int userId, int giftId)
    {
        try
        {
            var result = await _orderService.DeleteGiftFromDraftByUserAsync(userId, giftId);
            if (result) return Ok(true);
            return NotFound("Gift not found in draft");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
public class GiftUpdateDto
{
    public int UserId { get; set; }
    public int GiftId { get; set; }
    public int Amount { get; set; } 
}