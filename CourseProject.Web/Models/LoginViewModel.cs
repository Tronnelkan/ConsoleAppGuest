using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логін обов'язковий")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
