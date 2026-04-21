using FluentValidation;
using Hostel_Consume_2026.Models;
using System;

namespace Hostel_Consume_2026.Validators
{
    public class StudentAttendanceValidator : AbstractValidator<StudentAttendanceModel>
    {
        public StudentAttendanceValidator()
        {
            // Student Required
            RuleFor(a => a.Student_id)
                .NotEmpty().WithMessage("Student is required")
                .GreaterThan(0).WithMessage("Invalid Student");


            // Attendance Date Required and not future
            RuleFor(a => a.AttendanceDate)
                .NotEmpty().WithMessage("Attendance Date is required");
            //    .LessThanOrEqualTo(DateTime.Today)
            //    .WithMessage("Attendance Date cannot be future date");


            // Status Required
            RuleFor(a => a.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(20)
                .WithMessage("Status cannot exceed 20 characters");


            // Place Required
            RuleFor(a => a.Place)
                .NotEmpty().WithMessage("Place is required")
                .MaximumLength(50)
                .WithMessage("Place cannot exceed 50 characters");


            // Remarks Optional
            RuleFor(a => a.Remarks)
                .MaximumLength(200)
                .When(a => !string.IsNullOrEmpty(a.Remarks))
                .WithMessage("Remarks cannot exceed 200 characters");
        }
    }
}