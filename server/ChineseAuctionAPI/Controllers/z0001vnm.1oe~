using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Services;
using ChineseAuctionAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChineseAuctionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;
        private readonly IUserRepo _userRepo;
        private readonly ILogger<GiftController> _logger;

        public GiftController(IGiftService giftService, IUserRepo userRepo, ILogger<GiftController> logger)
        {
            _giftService = giftService;
            _userRepo = userRepo;
            _logger = logger;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("לא נבחר קובץ");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "public", "images", "gift");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileName = file.FileName });
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all gifts.");
                var gifts = await _giftService.GetAllGiftsAsync();
                return Ok(gifts);
            }
            catch (Exception ex)
            {
             
                _logger.LogError(ex, "An error occurred while fetching all gifts.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve gift with ID: {Id}", id);
                var gift = await _giftService.GetGiftByIdAsync(id);
                // return the gift (owner/user info is included by repository when available)
                return Ok(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching gift with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Post([FromBody] GiftDTO dto)
        {
            try
            {
                _logger.LogInformation("Attempting to create a new gift.");
                var newGift = await _giftService.CreateGiftAsync(dto);
                _logger.LogInformation("Gift created successfully with ID: {Id}", newGift.IdGift);

                return CreatedAtAction(nameof(Get), new { id = newGift.IdGift }, newGift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new gift.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete gift with ID: {Id}", id);
                var success = await _giftService.DeleteGiftAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Delete failed. Gift with ID: {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Gift with ID: {Id} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting gift with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPost("{id}/draw")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DrawWinner(int id)
        {
            var result = await _giftService.DrawWinnerForGiftAsync(id);

            if (result == null)
            {
                return BadRequest("לא ניתן לבצע הגרלה: המתנה לא נמצאה, כבר הוגרלה או שאין רוכשים.");
            }

            var gift = await _giftService.GetGiftByIdAsync(id);
            var winnerUser = gift?.OrdersGifts?.FirstOrDefault(og => og.Order?.IdUser == result.IdUser)?.Order?.User;
            if (winnerUser == null)
            {
                winnerUser = await _userRepo.GetByIdAsync(result.IdUser);
            }

            string fullName = winnerUser != null
                ? $"{winnerUser.FirstName} {winnerUser.LastName}"
                : $"משתמש {result.IdUser}";

            var response = new {
                gift = gift?.Name ?? string.Empty,
                winnerName = fullName,
                winnerUserId = result.IdUser,
                winnerEmail = winnerUser?.Email ?? string.Empty
            };

            _logger.LogInformation("הגרלה הסתיימה למתנה {Id}. זוכה: {FullName}", id, fullName);

            return Ok(response);
        }


        [HttpGet("sort/word/{word}")]

        public async Task<IActionResult> GetByNameGift(string word)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve gift with word: {word}", word);
                var gift = await _giftService.GetByNameGift(word);

                if (gift == null)
                {
                    _logger.LogWarning("Gift with word: {word} not found.", word);
                   
                }
                return Ok(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching gift with word: {word}", word);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("sort/donor/{donor}")]
        public async Task<IActionResult> GetByNameDonor(string donor)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve gift with donor: {donor}", donor);
                var gift = await _giftService.GetByNameDonor(donor);

                if (gift == null)
                {
                    _logger.LogWarning("Gift with donor: {donor} not found.", donor);

                }
                return Ok(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching gift with donor: {donor}", donor);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("sort/buyers/{buyers}")]
        public async Task<IActionResult> GetByNumOfBuyers(int buyers)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve gift with buyers: {buyers}", buyers);
                var gift = await _giftService.GetByNumOfBuyers(buyers);

                if (gift == null)
                {
                    _logger.LogWarning("Gift with buyers: {buyers} not found.", buyers);

                }
                return Ok(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching gift with buyers: {buyers}", buyers);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/participants")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetParticipants(int id)
        {
            try
            {
                _logger.LogInformation("שולף פרטי משתתפים מלאים עבור מתנה: {Id}", id);

                // שימוש בפונקציה המעודכנת מה-Service
                var participants = await _giftService.GetParticipantsDetailsAsync(id);

                if (participants == null || !participants.Any())
                {
                    _logger.LogWarning("לא נמצאו משתתפים למתנה {Id}", id);
                    return Ok(new List<ParticipantDetailsDTO>());
                }

                return Ok(participants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה ב-Endpoint שליפת פרטי משתתפים למתנה {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        //[HttpGet("{id}/participants")]
        //[Authorize(Roles = "Manager")] 
        //public async Task<IActionResult> GetParticipants(int id)
        //{
        //    try
        //    {
        //        _logger.LogInformation("שולף רשימת משתתפים עבור מתנה: {Id}", id);

        //        var names = await _giftService.GetParticipantsNamesAsync(id);

        //        if (names == null || !names.Any())
        //        {
        //            _logger.LogWarning("לא נמצאו משתתפים למתנה {Id}", id);
        //            return Ok(new List<string>()); 
        //        }

        //        return Ok(names);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "שגיאה ב-Endpoint שליפת משתתפים למתנה {Id}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        [HttpGet("winners")]
        public async Task<IActionResult> GetWinners()
        {
            try
            {
                var winners = await _giftService.GetAllWinnersAsync();
                var dto = winners.Select(w => new {
                    gift = w.Gift?.Name ?? "",
                    winnerName = w.User != null ? ($"{w.User.FirstName} {w.User.LastName}") : ($"משתמש {w.IdUser}"),
                    giftId = w.IdGift,
                    winnerUserId = w.IdUser
                });

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning winners list");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("sort/gift_price/")]
        public async Task<IActionResult> GetByNumOfBuyers()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve gift with Price: {Price}");
                var gift = await _giftService.SortByAmountPeople();

                if (gift == null)
                {
                    _logger.LogWarning("Gift with buyers: {buyers} not found.");

                }
                return Ok(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching gift with buyers: {buyers}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("sort/amount_buyers/")]
        public async Task<IActionResult> SortByAmountPeople()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve gift with Price: {Price}");
                var gift = await _giftService.SortByAmountPeople();
                if (gift == null)
                {
                    _logger.LogWarning("Gift with buyers: {buyers} not found.");

                }
                return Ok(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching gift with buyers: {buyers}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] GiftDTO dto)
        {
            try
            {
                _logger.LogInformation("Attempting to update gift with ID: {Id}", id);

                var success = await _giftService.UpdateGiftAsync(id, dto);

                if (!success)
                {
                    _logger.LogWarning("Update failed. Gift with ID: {Id} not found.", id);
                    return NotFound();
                }

                return NoContent(); // מחזיר קוד 204 (הצלחה)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating gift with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        }
}