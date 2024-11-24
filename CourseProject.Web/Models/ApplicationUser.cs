// Domain/Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Додаткові властивості
        [Required]
        public string FullName { get; set; }

        // Navigation property
        public ICollection<Role> Roles { get; set; }
    }
}
