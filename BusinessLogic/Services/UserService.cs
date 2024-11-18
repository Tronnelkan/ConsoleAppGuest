using BusinessLogic.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new();

        public Task<bool> AuthenticateUserAsync(string login, string password)
        {
            return Task.FromResult(_users.Any(u => u.Login == login && u.PasswordHash == password));
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

        public Task<User> GetUserByLoginAsync(string login)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Login == login));
        }
    }
}
