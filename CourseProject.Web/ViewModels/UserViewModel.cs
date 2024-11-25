using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseProject.Web.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Roles = new List<SelectListItem>();
        }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Логін є обов'язковим.")]
        [Display(Name = "Логін")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим.")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Прізвище є обов'язковим.")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; }

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

        [Required(ErrorMessage = "Роль є обов'язковою.")]
        [Display(Name = "Роль")]
        public string RoleName { get; set; }

        // Додано для випадаючого списку ролей
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
