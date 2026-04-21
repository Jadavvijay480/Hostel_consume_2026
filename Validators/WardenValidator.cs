using FluentValidation;
using Hostel_Consume_2026.Models;
using System;

namespace Hostel_Consume_2026.Validators
{
    public class WardenValidator : AbstractValidator<WardenModel>
    {
        public WardenValidator()
        {
            // First Name Required
            RuleFor(w => w.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(50)
                .WithMessage("First Name cannot exceed 50 characters");


            // Last Name Required
            RuleFor(w => w.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MaximumLength(50)
                .WithMessage("Last Name cannot exceed 50 characters");


            // Gender Required
            RuleFor(w => w.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .MaximumLength(10)
                .WithMessage("Gender cannot exceed 10 characters");


            // Phone Required and 10 digits
            RuleFor(w => w.Phone)
                .NotEmpty().WithMessage("Phone Number is required")
                .Matches(@"^\d{10}$")
                .WithMessage("Phone Number must be 10 digits");


            // Email Required
            RuleFor(w => w.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Address")
                .MaximumLength(100)
                .WithMessage("Email cannot exceed 100 characters");


            // Address Required
            RuleFor(w => w.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(255)
                .WithMessage("Address cannot exceed 255 characters");


            // Joining Date Required and not future
            RuleFor(w => w.JoiningDate)
                .NotEmpty().WithMessage("Joining Date is required");
            //    .LessThanOrEqualTo(DateTime.Today)
            //    .WithMessage("Joining Date cannot be future date");


            // Experience Required
            RuleFor(w => w.ExperienceYears)
                .NotEmpty().WithMessage("Experience Years is required")
                .InclusiveBetween(0, 50)
                .WithMessage("Experience must be between 0 and 50 years");


            // Status Required
            RuleFor(w => w.Status)
                .NotNull().WithMessage("Status is required");
        }
    }
}