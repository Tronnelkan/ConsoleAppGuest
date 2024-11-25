using CourseProject.DAL.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAddressRepository _addressRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                if (user == null) continue;

                // Завантаження адреси
                if (user.AddressId > 0)
                {
                    try
                    {
                        var address = await _addressRepository.GetByIdAsync(user.AddressId);
                        user.AddressEntity = address;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while fetching address with ID {user.AddressId}: {ex.Message}");
                    }
                }

                // Завантаження ролі
                if (user.RoleId > 0)
                {
                    try
                    {
                        var role = await _roleRepository.GetByIdAsync(user.RoleId);
                        user.Role = role;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while fetching role with ID {user.RoleId}: {ex.Message}");
                    }
                }
            }

            return users;
        }


        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await _userRepository.GetByLoginAsync(login);
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            // Перевірка наявності користувача з таким же логіном або Email
            var existingUser = await _userRepository.GetByLoginAsync(user.Login);
            if (existingUser != null)
                throw new ArgumentException("Користувач з таким логіном вже існує.");

            var existingEmail = await _userRepository.GetByEmailAsync(user.Email);
            if (existingEmail != null)
                throw new ArgumentException("Користувач з таким Email вже існує.");

            // Хешування пароля
            user.PasswordHash = ComputeHash(password);

            // Використання Recovery Keyword, введеного користувачем
            if (string.IsNullOrWhiteSpace(user.RecoveryKeyword))
                throw new ArgumentException("Recovery Keyword не може бути порожнім.");

            if (user.RoleId == 0) // Якщо RoleId не передано
            {
                var guestRole = await _roleRepository.GetByNameAsync("Guest");
                if (guestRole == null)
                {
                    // Якщо роль "Guest" відсутня, створіть її
                    guestRole = new Role { RoleName = "Guest" };
                    await _roleRepository.AddAsync(guestRole);
                    await _roleRepository.SaveChangesAsync();
                }
                user.RoleId = guestRole.RoleId;
            }

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<bool> AuthenticateAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
            {
                Console.WriteLine($"Користувач з логіном {login} не знайдений.");
                return false;
            }

            var hashedPassword = ComputeHash(password);
            if (user.PasswordHash == hashedPassword)
            {
                Console.WriteLine($"Користувач {login} успішно автентифікований.");
                return true;
            }
            else
            {
                Console.WriteLine($"Невірний пароль для користувача {login}.");
                return false;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<bool> ResetPasswordAsync(string email, string recoveryKeyword, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim());
            if (user == null)
                return false;

            // Перевірка Recovery Keyword
            if (!string.Equals(user.RecoveryKeyword, recoveryKeyword.Trim(), StringComparison.Ordinal))
                return false;

            // Хешування нового пароля
            user.PasswordHash = ComputeHash(newPassword);

            _userRepository.Update(user);
            try
            {
                await _userRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Логування помилки
                Console.WriteLine($"[ResetPasswordAsync] Error: {ex.Message}");
                throw new InvalidOperationException("An error occurred while saving the entity changes.", ex);
            }

            return true;
        }

        public async Task UpdateUserAsync(User user)
        {
            // Переконайтесь, що RecoveryKeyword не змінюється
            var existingUser = await _userRepository.GetByIdAsync(user.UserId);
            if (existingUser != null)
            {
                existingUser.Login = user.Login;
                existingUser.Email = user.Email;
                // Не змінюйте RecoveryKeyword та інші критичні поля
                // Наприклад:
                // existingUser.Role = user.Role;
                // existingUser.Address = user.Address;

                await _userRepository.UpdateUserAsync(existingUser);
            }
            else
            {
                throw new InvalidOperationException("Користувач не знайдений.");
            }
        }




        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }


        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Користувач не знайдений.");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public async Task<User> AddUserAsync(User user)
        {
            // Перевірка наявності користувача
            var existingUser = await _userRepository.GetByLoginAsync(user.Login);
            if (existingUser != null)
                throw new ArgumentException("Користувач з таким логіном вже існує.");

            var existingEmail = await _userRepository.GetByEmailAsync(user.Email);
            if (existingEmail != null)
                throw new ArgumentException("Користувач з таким Email вже існує.");

            // Хешування пароля, якщо це необхідно
            user.PasswordHash = ComputeHash("defaultPassword"); // або інший спосіб

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateRecoveryKeyword()
        {
            // Цей метод більше не потрібен, оскільки Recovery Keyword вводиться користувачем
            throw new NotImplementedException("Recovery Keyword is now user-provided.");
        }
    }
}
