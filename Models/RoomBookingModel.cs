using System;
using System.ComponentModel.DataAnnotations;

namespace Hostel_Consume_2026.Models
{
    public class RoomBookingModel
    {
        public int Booking_Id { get; set; }

        [Required(ErrorMessage = "Guest Name is required")]
        [StringLength(100, ErrorMessage = "Guest Name cannot exceed 100 characters")]
        [Display(Name = "Guest Name")]
        public string Guest_Name { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [StringLength(15, ErrorMessage = "Mobile Number cannot exceed 15 digits")]
        [Display(Name = "Mobile Number")]
        public string Mobile_No { get; set; }

        [Required(ErrorMessage = "Room Number is required")]
        [StringLength(20, ErrorMessage = "Room Number cannot exceed 20 characters")]
        [Display(Name = "Room Number")]
        public string Room_Number { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(20, ErrorMessage = "Address cannot exceed 20 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Booking Date is required")]
        [Display(Name = "Booking Date")]
        public DateTime Booking_Date { get; set; }

        [StringLength(50)]
        [Display(Name = "Booking Status")]
        public string Booking_Status { get; set; } = "Booked";

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
    }
}