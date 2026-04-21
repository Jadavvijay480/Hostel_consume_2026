using System;
using System.ComponentModel.DataAnnotations;

namespace Hostel_Consume_2026.Models
{
    public class WardenModel
    {
        [Key]
        public int Warden_id { get; set; }


        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Address is required")]
        [StringLength(255)]
        [Display(Name = "Address")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Joining Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }


        [Required(ErrorMessage = "Experience Years is required")]
        [Display(Name = "Experience Years")]
        public int ExperienceYears { get; set; }


        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public bool Status { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}