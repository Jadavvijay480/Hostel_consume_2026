 using System;
 using System.ComponentModel.DataAnnotations;

    namespace Hostel_Consume_2026.Models
    {
        public class StudentModel
        {
            public int Student_id { get; set; }

            [Required(ErrorMessage = "First Name is required")]
            [Display(Name = "First Name")]
            public string First_name { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            [Display(Name = "Last Name")]
            public string Last_name { get; set; }

            [Required(ErrorMessage = "Gender is required")]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Date of Birth is required")]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime Dob { get; set; }

            [Required(ErrorMessage = "Phone is required")]
            [Phone(ErrorMessage = "Invalid phone number")]
            [Display(Name = "Phone Number")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Address is required")]
            [Display(Name = "Address")]
            public string Address { get; set; }

            // Not stored in DB
            [Display(Name = "Full Name")]
            public string FullName => $"{First_name} {Last_name}";
        }

        public class StudentDropDownModel
        {
            public int Student_id { get; set; }

            [Display(Name = "Student Name")]
            public string StudentName { get; set; }
        }
    }

