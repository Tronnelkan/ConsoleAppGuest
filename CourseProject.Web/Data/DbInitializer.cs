using CourseProject.DAL.Repositories;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CourseProject.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleRepository = serviceProvider.GetRequiredService<IRoleRepository>();
            var addressRepository = serviceProvider.GetRequiredService<IAddressRepository>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            // Перевірка та створення ролі "Guest"
            var guestRole = await roleRepository.GetByIdAsync(1);
            if (guestRole == null)
            {
                guestRole = new Role
                {
                    RoleId = 1,
                    RoleName = "Guest"
                };
                await roleRepository.AddAsync(guestRole);
                await roleRepository.SaveChangesAsync();
            }

            // Перевірка та створення ролі "Admin"
            var adminRole = await roleRepository.GetByIdAsync(2);
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    RoleId = 2,
                    RoleName = "Admin"
                };
                await roleRepository.AddAsync(adminRole);
                await roleRepository.SaveChangesAsync();
            }

            // Перевірка та створення стандартної адреси
            var defaultAddress = await addressRepository.GetByIdAsync(1);
            if (defaultAddress == null)
            {
                defaultAddress = new Address
                {
                    AddressId = 1,
                    Street = "123 Default St",
                    City = "Default City",
                    Country = "Default Country"
                };
                await addressRepository.AddAsync(defaultAddress);
                await addressRepository.SaveChangesAsync();
            }

            // Перевірка та створення адміністратора
            var adminUser = await userRepository.GetByLoginAsync("admin");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    Login = "admin",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Gender = "Male",
                    Phone = "1234567890",
                    BankCardData = "1234567890123456",
                    RoleId = adminRole.RoleId,
                    AddressId = defaultAddress.AddressId,
                    RecoveryKeyword = "admin123" // Задайте відповідне значення або зробіть поле вводу
                };
                // Хешування пароля
                adminUser.PasswordHash = ComputeHash("Admin@123");
                await userRepository.AddAsync(adminUser);
                await userRepository.SaveChangesAsync();
            }
        }

        private static string ComputeHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                var builder = new System.Text.StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
