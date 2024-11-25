// CourseProject.Web/ViewModels/AddressViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.ViewModels
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Вулиця є обов'язковою.")]
        [Display(Name = "Вулиця")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Місто є обов'язковим.")]
        [Display(Name = "Місто")]
        public string City { get; set; }

        [Required(ErrorMessage = "Країна є обов'язковою.")]
        [Display(Name = "Країна")]
        public string Country { get; set; }

        // Навігаційні властивості, якщо необхідно
        // Наприклад:
        // public ICollection<UserViewModel> Users { get; set; }
    }
}
