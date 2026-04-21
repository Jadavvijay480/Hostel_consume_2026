using FluentValidation;
using Hostel_Consume_2026.Models;
using System;

namespace Hostel_Consume_2026.Validators
{
    public class FeesValidator : AbstractValidator<FeesModel>
    {
        public FeesValidator()
        {
            // Student Required
            RuleFor(f => f.Student_id)
                .NotEmpty().WithMessage("Student is required")
                .GreaterThan(0).WithMessage("Invalid Student");


            // Amount Required and Valid Range
            RuleFor(f => f.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .InclusiveBetween(1, 999999)
                .WithMessage("Amount must be between 1 and 999999");


            // Fee Month Required
            RuleFor(f => f.Fee_month)
                .NotEmpty().WithMessage("Fee Month is required")
                .MaximumLength(20)
                .WithMessage("Fee Month cannot exceed 20 characters");


            // Due Date Required and Not Past
            RuleFor(f => f.Due_date)
                .NotEmpty().WithMessage("Due Date is required");
                //.GreaterThanOrEqualTo(DateTime.Today)
                //.WithMessage("Due Date cannot be in the past");


            // Status Optional but if entered limit length
            RuleFor(f => f.Status)
                .MaximumLength(20)
                .When(f => !string.IsNullOrEmpty(f.Status))
                .WithMessage("Status cannot exceed 20 characters");
        }
    }
}