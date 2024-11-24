using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.Models
{
    public class RoleViewModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Назва ролі обов'язкова")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Назва ролі повинна містити від 3 до 50 символів")]
        public string RoleName { get; set; }
    }
}
