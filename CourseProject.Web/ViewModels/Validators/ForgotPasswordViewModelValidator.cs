// CourseProject.Web/ViewModels/Validators/ForgotPasswordViewModelValidator.cs
using FluentValidation;
using CourseProject.Web.ViewModels;

namespace CourseProject.Web.ViewModels.Validators
{
    public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordViewModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим.")
                .EmailAddress().WithMessage("Невірний формат Email.");

            RuleFor(x => x.RecoveryKeyword)
                .NotEmpty().WithMessage("Recovery Keyword є обов'язковим.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Новий пароль є обов'язковим.")
                .MinimumLength(6).WithMessage("Пароль повинен містити мінімум 6 символів.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword).WithMessage("Паролі не співпадають.");
        }
    }
}
