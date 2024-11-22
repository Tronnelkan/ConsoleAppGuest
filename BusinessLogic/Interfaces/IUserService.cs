// BusinessLogic/Interfaces/IUserService.cs
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task RegisterUserAsync(User user, string password);
        Task AddAddressAsync(Address address);
        Task<bool> RecoverPasswordAsync(string email, string recoveryKeyword, string newPassword);
    }
}
