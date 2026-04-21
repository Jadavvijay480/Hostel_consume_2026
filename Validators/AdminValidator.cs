using FluentValidation;
using Hostel_Consume_2026.Models;

namespace Hostel_Consume_2026.Validators
{
    public class AdminValidator : AbstractValidator<AdminModel>
    {
        public AdminValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z]+$").WithMessage("First Name must contain only letters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z]+$").WithMessage("Last Name must contain only letters");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .Must(g => g == "Male" || g == "Female" || g == "Other")
                .WithMessage("Invalid Gender value");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^[6-9][0-9]{9}$")
                .WithMessage("Phone must be a valid 10-digit Indian number");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email format")
                .MaximumLength(100);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MinimumLength(10).WithMessage("Address must be at least 10 characters");

            RuleFor(x => x.JoiningDate)
                .NotEmpty().WithMessage("Joining Date is required");
                //.LessThanOrEqualTo(DateTime.Today)
                //.WithMessage("Joining Date cannot be in the future");

            RuleFor(x => x.ExperienceYears)
                .NotNull().WithMessage("Experience is required")
                .InclusiveBetween(0, 50)
                .WithMessage("Experience must be between 0 and 50 years");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status is required");

            // Optional: Cross-field validation
            RuleFor(x => x)
                .Must(x => x.ExperienceYears <= (DateTime.Today.Year - x.JoiningDate.Year))
                .WithMessage("Experience cannot exceed total working years since joining");
        }
    }
}