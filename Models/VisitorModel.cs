using System;
using System.ComponentModel.DataAnnotations;

namespace Hostel_Consume_2026.Models
{
    public class VisitorModel
    {
        public int Visitor_id { get; set; }


        [Required(ErrorMessage = "Visitor Name is required")]
        [StringLength(100, ErrorMessage = "Visitor Name cannot exceed 100 characters")]
        [Display(Name = "Visitor Name")]
        public string VisitorName { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Visit Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Visit Date")]
        public DateTime VisitDate { get; set; }


        [Required(ErrorMessage = "In Time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "In Time")]
        public TimeSpan InTime { get; set; }


        [DataType(DataType.Time)]
        [Display(Name = "Out Time")]
        public TimeSpan? OutTime { get; set; }


        [Required(ErrorMessage = "Purpose is required")]
        [Display(Name = "Purpose")]
        public string Purpose { get; set; }


        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int Student_id { get; set; }


        [Display(Name = "Warden")]
        public int? Warden_id { get; set; }


        [Display(Name = "Status")]
        public bool Status { get; set; } = true;


        public DateTime CreatedAt { get; set; }
    }
}