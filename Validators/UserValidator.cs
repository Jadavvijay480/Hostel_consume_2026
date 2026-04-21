using FluentValidation;
using Hostel_Consume_2026.Models;

namespace Hostel_Consume_2026.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator()
        {
            // Full Name Required
            RuleFor(u => u.FullName)
                .NotEmpty().WithMessage("Full Name is required")
                .MaximumLength(100)
                .WithMessage("Full Name cannot exceed 100 characters");


            // Email Required and Valid
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Address")
                .MaximumLength(100)
                .WithMessage("Email cannot exceed 100 characters");


            // Password Required
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters")
                .MaximumLength(100)
                .WithMessage("Password cannot exceed 100 characters");


            // Mobile Required and 10 digits
            RuleFor(u => u.MobileNo)
                .NotEmpty().WithMessage("Mobile Number is required")
                .Matches(@"^\d{10}$")
                .WithMessage("Mobile Number must be 10 digits");


            // Role Required
            RuleFor(u => u.Role)
                .NotEmpty().WithMessage("Role is required")
                .MaximumLength(20)
                .WithMessage("Role cannot exceed 20 characters");


            // IsActive Required
            RuleFor(u => u.IsActive)
                .NotNull().WithMessage("Status is required");
        }
    }
}