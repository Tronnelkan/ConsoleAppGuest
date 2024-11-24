using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Models; // Переконайтеся, що цей using правильний
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Web.Data
{
    public class ApplicationIdentityContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationIdentityContext(DbContextOptions<ApplicationIdentityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Додаткові налаштування, якщо необхідно
        }
    }
}
