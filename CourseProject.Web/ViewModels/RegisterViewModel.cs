// CourseProject.Web/ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Логін є обов'язковим.")]
        [Display(Name = "Логін")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим.")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Прізвище є обов'язковим.")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Ключове слово для відновлення пароля є обов'язковим.")]
        [Display(Name = "Ключове слово для відновлення пароля")]
        public string RecoveryKeyword { get; set; }

        [Required(ErrorMessage = "Стать є обов'язковою.")]
        [Display(Name = "Стать")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Невірний формат Email.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон є обов'язковим.")]
        [Phone(ErrorMessage = "Невірний формат телефону.")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Адреса є обов'язковою.")]
        [Display(Name = "Адреса")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Дані банківської картки є обов'язковими.")]
        [Display(Name = "Дані банківської картки")]
        public string BankCardData { get; set; }
    }
}
