// DataAccess/Interfaces/IAddressRepository.cs
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address> GetByIdAsync(int id);
        Task CreateAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string street, string city, string country);
    }
}
