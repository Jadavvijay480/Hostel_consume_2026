using FluentValidation;
using Hostel_Consume_2026.Models;
using System;

namespace Hostel_Consume_2026.Validators
{
    public class VisitorValidator : AbstractValidator<VisitorModel>
    {
        public VisitorValidator()
        {
            // Visitor Name Required
            RuleFor(v => v.VisitorName)
                .NotEmpty().WithMessage("Visitor Name is required")
                .MaximumLength(100)
                .WithMessage("Visitor Name cannot exceed 100 characters");


            // Gender Required
            RuleFor(v => v.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .MaximumLength(10)
                .WithMessage("Gender cannot exceed 10 characters");


            // Phone Required and 10 digits
            RuleFor(v => v.Phone)
                .NotEmpty().WithMessage("Phone Number is required")
                .Matches(@"^\d{10}$")
                .WithMessage("Phone Number must be 10 digits");


            // Email Optional but valid
            RuleFor(v => v.Email)
                .EmailAddress().WithMessage("Invalid Email Address")
                .When(v => !string.IsNullOrEmpty(v.Email));


            // Address Required
            RuleFor(v => v.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200)
                .WithMessage("Address cannot exceed 200 characters");


            // Visit Date Required and not future
            RuleFor(v => v.VisitDate)
                .NotEmpty().WithMessage("Visit Date is required");
            //    .LessThanOrEqualTo(DateTime.Today)
            //    .WithMessage("Visit Date cannot be future date");


            // In Time Required
            RuleFor(v => v.InTime)
                .NotEmpty().WithMessage("In Time is required");


            // Out Time must be after In Time
            RuleFor(v => v.OutTime)
                .GreaterThan(v => v.InTime)
                .When(v => v.OutTime.HasValue)
                .WithMessage("Out Time must be after In Time");


            // Purpose Required
            RuleFor(v => v.Purpose)
                .NotEmpty().WithMessage("Purpose is required")
                .MaximumLength(200)
                .WithMessage("Purpose cannot exceed 200 characters");


            // Student Required
            RuleFor(v => v.Student_id)
                .NotEmpty().WithMessage("Student is required")
                .GreaterThan(0).WithMessage("Invalid Student");


            // Warden Optional
            RuleFor(v => v.Warden_id)
                .GreaterThan(0)
                .When(v => v.Warden_id.HasValue)
                .WithMessage("Invalid Warden");
        }
    }
}