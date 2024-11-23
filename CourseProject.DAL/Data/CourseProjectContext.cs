using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.DAL.Data
{
    public class CourseProjectContext : DbContext
    {
        public CourseProjectContext(DbContextOptions<CourseProjectContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зв’язків
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.AddressEntity) // Замість u.Address
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
