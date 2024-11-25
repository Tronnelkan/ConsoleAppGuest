// CourseProject.Web/ViewModels/RoleViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.ViewModels
{
    public class RoleViewModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Назва ролі є обов'язковою.")]
        [StringLength(50, ErrorMessage = "Назва ролі не може перевищувати 50 символів.")]
        [Display(Name = "Назва ролі")]
        public string RoleName { get; set; }
    }
}
