using System.ComponentModel.DataAnnotations;

namespace Hostel_Consume_2026.Models
{
    public class RoomModel
    {
        public int Room_id { get; set; }


        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int Student_id { get; set; }


        [Required(ErrorMessage = "Hostel is required")]
        [Display(Name = "Hostel")]
        public int Hostel_id { get; set; }


        [Required(ErrorMessage = "Room Number is required")]
        [StringLength(10, ErrorMessage = "Room Number cannot exceed 10 characters")]
        [Display(Name = "Room Number")]
        public string Room_number { get; set; }


        [Required(ErrorMessage = "Room Type is required")]
        [StringLength(50, ErrorMessage = "Room Type cannot exceed 50 characters")]
        [Display(Name = "Room Type")]
        public string Room_type { get; set; }   // Single, Double, Dorm


        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }


        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Available";
    }
}