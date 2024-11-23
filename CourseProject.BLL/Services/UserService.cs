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
            return await _userRepository.GetAllAsync();
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

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<bool> AuthenticateAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null) return false;

            return VerifyPassword(password, user.PasswordHash);
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
