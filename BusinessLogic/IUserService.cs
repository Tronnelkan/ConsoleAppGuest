using Domain.Models;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(User user);
        Task<User> AuthenticateUserAsync(string login, string password);
        Task<bool> RecoverPasswordAsync(string login, string recoveryKeyword, string newPassword);
    }
}
