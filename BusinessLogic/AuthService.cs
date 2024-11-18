using DataAccess.Interfaces;
using Domain.Models;

namespace BusinessLogic.Services
{
    public interface IAuthService
    {
        bool Authenticate(string login, string password);
        string GetUserRole(string login);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Authenticate(string login, string password)
        {
            var user = _userRepository.GetByLoginAsync(login).Result;
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public string GetUserRole(string login)
        {
            var user = _userRepository.GetByLoginAsync(login).Result;
            return user?.Role?.RoleName ?? "Guest";
        }
    }
}
