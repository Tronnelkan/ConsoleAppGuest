using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Domain.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByLoginAsync(user.Login);
            if (existingUser != null)
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _userRepository.CreateAsync(user);
            return true;
        }

        public async Task<User> AuthenticateUserAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public async Task<bool> RecoverPasswordAsync(string login, string recoveryKeyword, string newPassword)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user != null && user.RecoveryKeyword == recoveryKeyword)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _userRepository.UpdateAsync(user);
                return true;
            }
            return false;
        }
    }
}
