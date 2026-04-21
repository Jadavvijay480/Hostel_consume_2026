using System;
using System.ComponentModel.DataAnnotations;

namespace Hostel_Consume_2026.Models
{
    public class StudentAttendanceModel
    {
        public int Attendance_id { get; set; }


        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int Student_id { get; set; }


        [Required(ErrorMessage = "Attendance Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; }


        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }   // Present, Absent


        [Required(ErrorMessage = "Place is required")]
        [Display(Name = "Place")]
        public string Place { get; set; }    // Hostel, Outing


        [Display(Name = "Remarks")]
        public string Remarks { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}