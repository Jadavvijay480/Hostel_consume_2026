namespace Hostel_Consume_2026.Models
{
    public class DashboardViewModel
    {
        public int StudentCount { get; set; }
        public int WardenCount { get; set; }
        public int VisitorCount { get; set; }

        public int PaidFeesCount { get; set; }
        public int TotalFeesCount { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public int StudentPercent { get; set; }
        public int WardenPercent { get; set; }
        public int VisitorPercent { get; set; }
        public int FeesPaidPercent { get; set; }
        public int FeesAmountPercent { get; set; }
    }
}
