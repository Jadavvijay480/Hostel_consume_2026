using FluentValidation;
using Hostel_Consume_2026.Models;

namespace Hostel_Consume_2026.Validators
{
    public class RoomValidator : AbstractValidator<RoomModel>
    {
        public RoomValidator()
        {
            // Student Required
            RuleFor(r => r.Student_id)
                .NotEmpty().WithMessage("Student is required")
                .GreaterThan(0).WithMessage("Invalid Student");


            // Hostel Required
            RuleFor(r => r.Hostel_id)
                .NotEmpty().WithMessage("Hostel is required")
                .GreaterThan(0).WithMessage("Invalid Hostel");


            // Room Number Required
            RuleFor(r => r.Room_number)
                .NotEmpty().WithMessage("Room Number is required")
                .MaximumLength(10)
                .WithMessage("Room Number cannot exceed 10 characters");


            // Room Type Required
            RuleFor(r => r.Room_type)
                .NotEmpty().WithMessage("Room Type is required")
                .MaximumLength(50)
                .WithMessage("Room Type cannot exceed 50 characters");


            // Capacity Required and Range
            RuleFor(r => r.Capacity)
                .NotEmpty().WithMessage("Capacity is required")
                .InclusiveBetween(1, 20)
                .WithMessage("Capacity must be between 1 and 20");


            // Status Optional
            RuleFor(r => r.Status)
                .MaximumLength(20)
                .When(r => !string.IsNullOrEmpty(r.Status))
                .WithMessage("Status cannot exceed 20 characters");
        }
    }
}