// Domain/Models/Role.cs
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }

        // Navigation property
        public ICollection<User> Users { get; set; }
    }
}
