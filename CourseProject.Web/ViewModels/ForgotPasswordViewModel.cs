// CourseProject.Web/ViewModels/ForgotPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Невірний формат Email.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Recovery Keyword є обов'язковим.")]
        [Display(Name = "Recovery Keyword")]
        public string RecoveryKeyword { get; set; }

        [Required(ErrorMessage = "Новий пароль є обов'язковим.")]
        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають.")]
        public string ConfirmPassword { get; set; }
    }
}
