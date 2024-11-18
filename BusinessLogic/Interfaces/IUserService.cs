using Domain.Models;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<bool> AuthenticateUserAsync(string username, string password);
        Task RegisterUserAsync(User user);
        Task<User> GetUserByUsernameAsync(string username);
    }
}
