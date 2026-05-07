using System.ComponentModel.DataAnnotations;

namespace ChineseAuctionAPI.DTOs
{
    public class WinnerDTO
    {
        [Required]
        public string GiftName { get; set; }
        [Required]
        public string WinnerFullName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "כתובת האימייל אינה תקינה")]
        public string WinnerEmail { get; set; }
    }

    public class GiftWithParticipantsDto
    {
        public int IdGift { get; set; }
        public string Name { get; set; }
        public int TotalTicketsSold { get; set; } // סך כל הכרטיסים שנמכרו למתנה זו
        public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();
    }

    public class ParticipantDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int TicketsCount { get; set; } // כמה כרטיסים המשתמש הספציפי הזה קנה
    }
}


