using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>();

        public Task<bool> AuthenticateUserAsync(string username, string password)
        {
            return Task.FromResult(_users.Any(u => u.Login == username && u.PasswordHash == password));
        }

        public Task RegisterUserAsync(User user)
        {
            if (_users.Any(u => u.Login == user.Login))
            {
                throw new Exception("User already exists.");
            }

            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Login == username));
        }

        public void ResetPassword(string username, string recoveryKeyword, string newPassword)
        {
            var user = _users.FirstOrDefault(u => u.Login == username);
            if (user == null || user.RecoveryKeyword != recoveryKeyword)
            {
                throw new Exception("Invalid username or recovery keyword.");
            }

            user.PasswordHash = newPassword;
        }
    }
}
