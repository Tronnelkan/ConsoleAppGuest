using CourseProject.BLL.Services;
using CourseProject.DAL.Repositories;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _addressRepository.GetAllAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await _addressRepository.GetByIdAsync(id);
        }

        public async Task<Address> GetAddressByFullAddressAsync(string fullAddress)
        {
            return await _addressRepository.GetByFullAddressAsync(fullAddress);
        }

        public async Task AddAddressAsync(Address address)
        {
            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveChangesAsync();
        }

        public async Task UpdateAddressAsync(Address address)
        {
            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(int id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address != null)
            {
                _addressRepository.Delete(address);
                await _addressRepository.SaveChangesAsync();
            }
        }
    }
}
