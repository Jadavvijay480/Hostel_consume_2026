namespace Hostel_Consume_2026.Models
{
    public class FeesViewModel
    {
        public int Fee_id { get; set; }

        // Student info
        public int Student_id { get; set; }
        public string Student_Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Fee info
        public decimal Amount { get; set; }
        public string Fee_month { get; set; }
        public DateTime Due_date { get; set; }
        public string Status { get; set; }

        // Optional: Room info
        public int? Room_id { get; set; }
        public string Room_number { get; set; }
        public string Room_type { get; set; }
    }
}
