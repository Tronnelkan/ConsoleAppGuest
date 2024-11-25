using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логін є обов'язковим.")]
        [Display(Name = "Логін")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }
}
