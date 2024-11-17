using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Interfaces;
using Domain.Models;
using BCrypt.Net;
using System.Linq;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace ConsoleApp1
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            await InitializeDatabaseAsync();
            await MainMenuAsync();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5433;Database=MICRO_DB;Username=postgres;Password=tatarinn"));

            services.AddDataAccessServices();
        }

        private static async Task InitializeDatabaseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();

            var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
            if (!await roleRepository.ExistsAsync("Guest"))
            {
                await roleRepository.CreateAsync(new Role { RoleName = "Guest" });
            }

            var addressRepository = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            if (!await addressRepository.ExistsAsync("Default St", "Default City", "Default Country"))
            {
                await addressRepository.CreateAsync(new Address { Street = "Default St", City = "Default City", Country = "Default Country" });
            }
        }

        private static async Task MainMenuAsync()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("=== Main Menu ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Recover Password");
                Console.WriteLine("4. CRUD Demo");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await RegisterAsync();
                        break;
                    case "2":
                        await LoginAsync();
                        break;
                    case "3":
                        await RecoverPasswordAsync();
                        break;
                    case "4":
                        await CrudDemoAsync();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static async Task RegisterAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
            var addressRepository = scope.ServiceProvider.GetRequiredService<IAddressRepository>();

            Console.WriteLine("=== Register ===");

            Console.Write("Login: ");
            string login = Console.ReadLine();

            var existingUser = await userRepository.GetByLoginAsync(login);
            if (existingUser != null)
            {
                Console.WriteLine("A user with this login already exists.");
                return;
            }

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Password: ");
            string password = ReadPassword();

            Console.Write("Confirm Password: ");
            string confirmPassword = ReadPassword();

            if (password != confirmPassword)
            {
                Console.WriteLine("Passwords do not match.");
                return;
            }

            Console.Write("Recovery Keyword: ");
            string recoveryKeyword = Console.ReadLine();

            Console.Write("Gender (Male/Female/Other): ");
            string gender = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Bank Card Data: ");
            string bankCardData = Console.ReadLine();

            Console.WriteLine("Enter Address:");
            Console.Write("Street: ");
            string street = Console.ReadLine();
            Console.Write("City: ");
            string city = Console.ReadLine();
            Console.Write("Country: ");
            string country = Console.ReadLine();

            var address = new Address
            {
                Street = street,
                City = city,
                Country = country
            };
            await addressRepository.CreateAsync(address);

            var guestRole = await roleRepository.GetByNameAsync("Guest");
            if (guestRole == null)
            {
                Console.WriteLine("Role 'Guest' not found.");
                return;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Login = login,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = passwordHash,
                RecoveryKeyword = recoveryKeyword,
                Gender = gender,
                Email = email,
                Phone = phone,
                BankCardData = bankCardData,
                RoleId = guestRole.RoleId,
                AddressId = address.AddressId
            };

            await userRepository.CreateAsync(user);
            Console.WriteLine("Registration successful.");
        }

        private static async Task LoginAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            Console.WriteLine("=== Login ===");

            Console.Write("Login: ");
            string login = Console.ReadLine();

            Console.Write("Password: ");
            string password = ReadPassword();

            var user = await userRepository.GetByLoginAsync(login);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isPasswordValid)
            {
                Console.WriteLine($"Welcome, {user.FirstName} {user.LastName}!");
                Console.WriteLine($"Your role: {user.Role?.RoleName ?? "Role not assigned"}");
            }
            else
            {
                Console.WriteLine("Invalid password.");
            }
        }

        private static async Task RecoverPasswordAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            Console.WriteLine("=== Recover Password ===");

            Console.Write("Login: ");
            string login = Console.ReadLine();

            var user = await userRepository.GetByLoginAsync(login);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            Console.Write("Recovery Keyword: ");
            string recoveryKeyword = Console.ReadLine();

            if (user.RecoveryKeyword != recoveryKeyword)
            {
                Console.WriteLine("Invalid recovery keyword.");
                return;
            }

            Console.Write("New Password: ");
            string newPassword = ReadPassword();

            Console.Write("Confirm New Password: ");
            string confirmPassword = ReadPassword();

            if (newPassword != confirmPassword)
            {
                Console.WriteLine("Passwords do not match.");
                return;
            }

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = newPasswordHash;

            await userRepository.UpdateAsync(user);
            Console.WriteLine("Password updated successfully.");
        }

        private static async Task CrudDemoAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            bool back = false;
            while (!back)
            {
                Console.WriteLine("=== CRUD Demo ===");
                Console.WriteLine("1. Create User");
                Console.WriteLine("2. View All Users");
                Console.WriteLine("3. Update User");
                Console.WriteLine("4. Delete User");
                Console.WriteLine("5. Return to Main Menu");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await CreateUserAsync(userRepository);
                        break;
                    case "2":
                        await ViewAllUsersAsync(userRepository);
                        break;
                    case "3":
                        await UpdateUserAsync(userRepository);
                        break;
                    case "4":
                        await DeleteUserAsync(userRepository);
                        break;
                    case "5":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static async Task CreateUserAsync(IUserRepository userRepository)
        {
            Console.WriteLine("=== Create User ===");

            Console.Write("Login: ");
            string login = Console.ReadLine();

            var existingUser = await userRepository.GetByLoginAsync(login);
            if (existingUser != null)
            {
                Console.WriteLine("A user with this login already exists.");
                return;
            }

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Password: ");
            string password = ReadPassword();

            Console.Write("Recovery Keyword: ");
            string recoveryKeyword = Console.ReadLine();

            Console.Write("Gender (Male/Female/Other): ");
            string gender = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Bank Card Data: ");
            string bankCardData = Console.ReadLine();

            var defaultAddress = (await userRepository.GetAllAsync()).FirstOrDefault()?.Address;
            if (defaultAddress == null)
            {
                Console.WriteLine("Default address not found.");
                return;
            }

            var role = (await userRepository.GetAllAsync()).FirstOrDefault()?.Role;
            if (role == null)
            {
                Console.WriteLine("Role not found.");
                return;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Login = login,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = passwordHash,
                RecoveryKeyword = recoveryKeyword,
                Gender = gender,
                Email = email,
                Phone = phone,
                BankCardData = bankCardData,
                RoleId = role.RoleId,
                AddressId = defaultAddress.AddressId
            };

            await userRepository.CreateAsync(user);
            Console.WriteLine("User created.");
        }

        private static async Task ViewAllUsersAsync(IUserRepository userRepository)
        {
            Console.WriteLine("=== User List ===");
            var users = await userRepository.GetAllWithRolesAsync();

            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.UserId}, Login: {user.Login}, First Name: {user.FirstName}, Last Name: {user.LastName}, Email: {user.Email}, Role: {user.Role?.RoleName ?? "Role not assigned"}");
            }
        }

        private static async Task UpdateUserAsync(IUserRepository userRepository)
        {
            Console.WriteLine("=== Update User ===");
            Console.Write("Enter the ID of the user to update: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            Console.WriteLine($"Updating user: {user.Login}");

            Console.Write("New Email (leave blank for no changes): ");
            string email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email))
            {
                user.Email = email;
            }

            Console.Write("New Phone (leave blank for no changes): ");
            string phone = Console.ReadLine();
            if (!string.IsNullOrEmpty(phone))
            {
                user.Phone = phone;
            }

            Console.Write("New Bank Card Data (leave blank for no changes): ");
            string bankCardData = Console.ReadLine();
            if (!string.IsNullOrEmpty(bankCardData))
            {
                user.BankCardData = bankCardData;
            }

            await userRepository.UpdateAsync(user);
            Console.WriteLine("User updated.");
        }

        private static async Task DeleteUserAsync(IUserRepository userRepository)
        {
            Console.WriteLine("=== Delete User ===");
            Console.Write("Enter the ID of the user to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            Console.Write($"Are you sure you want to delete user '{user.Login}'? (y/n): ");
            var confirmation = Console.ReadLine();
            if (confirmation.ToLower() == "y")
            {
                await userRepository.DeleteAsync(userId);
                Console.WriteLine("User deleted.");
            }
            else
            {
                Console.WriteLine("Operation canceled.");
            }
        }

        private static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return password;
        }
    }
}
