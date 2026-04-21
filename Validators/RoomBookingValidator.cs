using FluentValidation;
using Hostel_Consume_2026.Models;

namespace Hostel_Consume_2026.Validators
{
    public class RoomBookingValidator : AbstractValidator<RoomBookingModel>
    {
        public RoomBookingValidator()
        {
            // Guest Name Required
            RuleFor(r => r.Guest_Name)
                .NotEmpty().WithMessage("Guest Name is required")
                .MaximumLength(100)
                .WithMessage("Guest Name cannot exceed 100 characters");

            // Mobile Number Required
            RuleFor(r => r.Mobile_No)
                .NotEmpty().WithMessage("Mobile Number is required")
                .MaximumLength(15)
                .WithMessage("Mobile Number cannot exceed 15 digits");

            // Room Number Required
            RuleFor(r => r.Room_Number)
                .NotEmpty().WithMessage("Room Number is required")
                .MaximumLength(20)
                .WithMessage("Room Number cannot exceed 20 characters");

            // Address Required
            RuleFor(r => r.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(20)
                .WithMessage("Address cannot exceed 20 characters");

            // Booking Date Required
            RuleFor(r => r.Booking_Date)
                .NotEmpty().WithMessage("Booking Date is required");
                //.LessThanOrEqualTo(DateTime.Now)
                //.WithMessage("Booking Date cannot be in the future");

            // Booking Status Optional
            RuleFor(r => r.Booking_Status)
                .MaximumLength(50)
                .When(r => !string.IsNullOrEmpty(r.Booking_Status))
                .WithMessage("Booking Status cannot exceed 50 characters");
        }
    }
}