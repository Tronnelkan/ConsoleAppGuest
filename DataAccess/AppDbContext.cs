// DataAccess/AppDbContext.cs
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Настройка связей и ограничений
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка уникальности
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Login)
                        .IsUnique();

            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Email)
                        .IsUnique();

            // Настройка связей
            modelBuilder.Entity<User>()
                        .HasOne(u => u.Role)
                        .WithMany()
                        .HasForeignKey(u => u.RoleId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                        .HasOne(u => u.Address)
                        .WithMany()
                        .HasForeignKey(u => u.AddressId)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
