using System;
using System.ComponentModel.DataAnnotations;

namespace Hostel_Consume_2026.Models
{
    public class ComplaintModel
    {
        public int Complaint_id { get; set; }


        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int Student_id { get; set; }


        [Required(ErrorMessage = "Complaint Type is required")]
        [Display(Name = "Complaint Type")]
        public string ComplaintType { get; set; }


        [Required(ErrorMessage = "Complaint Details are required")]
        [Display(Name = "Complaint Details")]
        public string ComplaintDetails { get; set; }


        [Required(ErrorMessage = "Complaint Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Complaint Date")]
        public DateTime ComplaintDate { get; set; }


        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }


        [Display(Name = "Action Taken")]
        public string ActionTaken { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Resolved Date")]
        public DateTime? ResolvedDate { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}