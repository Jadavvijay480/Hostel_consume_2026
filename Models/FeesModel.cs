using System;
using System.ComponentModel.DataAnnotations;

namespace Hostel_Consume_2026.Models
{
    public class FeesModel
    {
        public int Fee_id { get; set; }


        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int Student_id { get; set; }


        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 999999, ErrorMessage = "Amount must be between 1 and 999999")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }


        [Required(ErrorMessage = "Fee Month is required")]
        [Display(Name = "Fee Month")]
        public string Fee_month { get; set; }


        [Required(ErrorMessage = "Due Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime Due_date { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";
    }
}