using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<Address> GetAddressByIdAsync(int id);
        Task<Address> GetAddressByFullAddressAsync(string fullAddress);
        Task AddAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
        Task DeleteAddressAsync(int id);
    }
}
