// BusinessLogic/Services/UserService.cs
using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;

namespace BusinessLogic.Services
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

        public async Task RegisterUserAsync(User user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Проверка наличия роли
            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            if (role == null)
                throw new InvalidOperationException("Selected role does not exist.");

            // Проверка наличия адреса
            var address = await _addressRepository.GetByIdAsync(user.AddressId);
            if (address == null)
                throw new InvalidOperationException("Selected address does not exist.");

            // Проверка наличия пользователя с таким логином
            var existingUser = await _userRepository.GetByLoginAsync(user.Login);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists.");

            // Хеширование пароля
            user.PasswordHash = HashPassword(password);

            // Генерация RecoveryKeyword
            user.RecoveryKeyword = GenerateRecoveryKeyword();

            await _userRepository.CreateAsync(user);
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByLoginAsync(username);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task AddAddressAsync(Address address)
        {
            // Проверка, существует ли адрес уже
            bool exists = await _addressRepository.ExistsAsync(address.Street, address.City, address.Country);
            if (!exists)
            {
                await _addressRepository.CreateAsync(address);
            }
            // Иначе, можно выбросить исключение или игнорировать
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByLoginAsync(username);
        }

        public async Task ResetPasswordAsync(string username, string newPassword, string recoveryKeyword)
        {
            var user = await _userRepository.GetByLoginAsync(username);
            if (user == null)
                throw new InvalidOperationException("User does not exist.");

            if (user.RecoveryKeyword != recoveryKeyword)
                throw new InvalidOperationException("Invalid recovery keyword.");

            user.PasswordHash = HashPassword(newPassword);
            // Возможно, вы хотите обновить RecoveryKeyword или другие поля
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> RecoverPasswordAsync(string username, string recoveryKeyword, string newPassword)
        {
            var user = await _userRepository.GetByLoginAsync(username);
            if (user == null || user.RecoveryKeyword != recoveryKeyword)
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _addressRepository.GetAllAsync();
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateRecoveryKeyword()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
