using FluentValidation;
using Hostel_Consume_2026.Models;
using System;

namespace Hostel_Consume_2026.Validators
{
    public class StudentValidator : AbstractValidator<StudentModel>
    {
        public StudentValidator()
        {
            // First Name - Required, max 25 chars
            RuleFor(s => s.First_name)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(25).WithMessage("First Name cannot exceed 25 characters");

            // Last Name - Required, max 25 chars
            RuleFor(s => s.Last_name)
                .NotEmpty().WithMessage("Last Name is required")
                .MaximumLength(25).WithMessage("Last Name cannot exceed 25 characters");

            // Gender - Required
            RuleFor(s => s.Gender)
                .NotEmpty().WithMessage("Gender is required");

            // Date of Birth - Required, must be in past
            RuleFor(s => s.Dob)
                .NotEmpty().WithMessage("Date of Birth is required");
            //    .LessThan(DateTime.Today).WithMessage("Date of Birth must be in the past");

            // Phone - Required, must be 10 digits
            RuleFor(s => s.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\d{10}$").WithMessage("Phone must be 10 digits");

            // Email - Required, valid email format
            RuleFor(s => s.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            // Address - Required, max 200 chars
            RuleFor(s => s.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");
        }
    }
}