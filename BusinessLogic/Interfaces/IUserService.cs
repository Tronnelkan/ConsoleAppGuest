// BusinessLogic/Interfaces/IUserService.cs
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(User user, string password);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<bool> RecoverPasswordAsync(string username, string recoveryKeyword, string newPassword);
        Task AddAddressAsync(Address address); // Добавлено
    }
}
