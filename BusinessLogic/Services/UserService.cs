using Domain.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>();

        // Перевірка аутентифікації
        public Task<bool> AuthenticateUserAsync(string username, string password)
        {
            return Task.FromResult(_users.Any(u => u.Username == username && u.PasswordHash == password));
        }

        // Реєстрація нового користувача
        public Task RegisterUserAsync(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
            {
                throw new Exception("User already exists.");
            }

            _users.Add(user);
            return Task.CompletedTask;
        }

        // Пошук користувача за ім'ям
        public Task<User> GetUserByUsernameAsync(string username)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
        }
    }
}
