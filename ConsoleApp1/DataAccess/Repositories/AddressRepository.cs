using System.Threading.Tasks;
using Domain.Models;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(string street, string city, string country)
        {
            return await _dbSet.AnyAsync(a => a.Street == street && a.City == city && a.Country == country);
        }
    }
}
