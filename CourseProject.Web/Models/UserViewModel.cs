using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Логін обов'язковий")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логін повинен містити від 3 до 50 символів")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Прізвище обов'язкове")]
        [StringLength(50, ErrorMessage = "Прізвище не може перевищувати 50 символів")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Стать обов'язкова")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон обов'язковий")]
        [Phone(ErrorMessage = "Некоректний формат телефону")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Дані банківської картки обов'язкові")]
        [StringLength(20, MinimumLength = 16, ErrorMessage = "Дані банківської картки повинні містити від 16 до 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Дані банківської картки повинні містити тільки цифри")]
        public string BankCardData { get; set; }

        // Додаткові властивості для відображення
        public string RoleName { get; set; }
        public string Address { get; set; }
    }
}
