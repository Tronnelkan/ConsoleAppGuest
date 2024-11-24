using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByLoginAsync(string login);
        Task<User> RegisterUserAsync(User user, string password);
        Task<bool> AuthenticateAsync(string login, string password);
        Task<bool> ResetPasswordAsync(string email, string recoveryKeyword, string newPassword);
        Task DeleteUserAsync(int userId);
        Task UpdateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
    }
}
