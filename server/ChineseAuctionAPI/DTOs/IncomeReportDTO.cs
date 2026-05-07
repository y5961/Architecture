namespace ChineseAuctionAPI.DTOs
{
    public class IncomeReportDTO
    {
        public decimal TotalRevenue { get; set; } 
        public int TotalBuyers { get; set; }    
        public int TotalDonors { get; set; }      
        public DateTime ReportDate { get; set; } = DateTime.Now;
    }
}