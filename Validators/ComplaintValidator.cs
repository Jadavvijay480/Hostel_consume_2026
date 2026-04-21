using FluentValidation;
using Hostel_Consume_2026.Models;
using System;

namespace Hostel_Consume_2026.Validators
{
    public class ComplaintValidator : AbstractValidator<ComplaintModel>
    {
        public ComplaintValidator()
        {
            // Student Required
            RuleFor(c => c.Student_id)
                .NotEmpty().WithMessage("Student is required")
                .GreaterThan(0).WithMessage("Invalid Student");


            // Complaint Type Required, Max 100 chars
            RuleFor(c => c.ComplaintType)
                .NotEmpty().WithMessage("Complaint Type is required")
                .MaximumLength(100).WithMessage("Complaint Type cannot exceed 100 characters");


            // Complaint Details Required, Max 500 chars
            RuleFor(c => c.ComplaintDetails)
                .NotEmpty().WithMessage("Complaint Details are required")
                .MaximumLength(500).WithMessage("Complaint Details cannot exceed 500 characters");


            //Complaint Date Required, must be today or past
            RuleFor(c => c.ComplaintDate)
                .NotEmpty().WithMessage("Complaint Date is required");
                //.LessThanOrEqualTo(DateTime.Today)
                //.WithMessage("Complaint Date cannot be future date");


            // Status Required
            RuleFor(c => c.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters");


            // Action Taken optional, max 500 chars
            RuleFor(c => c.ActionTaken)
                .MaximumLength(500)
                .When(c => !string.IsNullOrEmpty(c.ActionTaken))
                .WithMessage("Action Taken cannot exceed 500 characters");


            // Resolved Date must be after Complaint Date
            RuleFor(c => c.ResolvedDate)
                .GreaterThanOrEqualTo(c => c.ComplaintDate)
                .When(c => c.ResolvedDate.HasValue)
                .WithMessage("Resolved Date must be after Complaint Date");
        }
    }
}