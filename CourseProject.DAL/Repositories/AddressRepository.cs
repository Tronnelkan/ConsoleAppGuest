using CourseProject.DAL.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CourseProject.DAL.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(CourseProjectContext context) : base(context)
        {
        }

        public async Task<Address> GetByFullAddressAsync(string fullAddress)
        {
            var parts = fullAddress.Split(", ");
            if (parts.Length != 3)
                return null;

            var street = parts[0];
            var city = parts[1];
            var country = parts[2];

            return await _dbSet.FirstOrDefaultAsync(a => a.Street == street && a.City == city && a.Country == country);
        }
    }
}
