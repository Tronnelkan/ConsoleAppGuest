// CourseProject.Web/ViewModels/Validators/RegisterViewModelValidator.cs
using FluentValidation;
using CourseProject.Web.ViewModels;

namespace CourseProject.Web.ViewModels.Validators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Логін є обов'язковим.")
                .MaximumLength(50).WithMessage("Логін не може перевищувати 50 символів.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим.")
                .MinimumLength(6).WithMessage("Пароль повинен містити мінімум 6 символів.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Паролі не співпадають.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ім'я є обов'язковим.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Прізвище є обов'язковим.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим.")
                .EmailAddress().WithMessage("Невірний формат Email.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Телефон є обов'язковим.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Невірний формат телефону.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адреса є обов'язковою.");

            RuleFor(x => x.BankCardData)
                .NotEmpty().WithMessage("Дані банківської картки є обов'язковими.")
                .Matches(@"^\d{4}-\d{4}-\d{4}-\d{4}$").WithMessage("Невірний формат даних банківської картки.");

            RuleFor(x => x.RecoveryKeyword)
                .NotEmpty().WithMessage("Recovery Keyword є обов'язковим.");
        }
    }
}
