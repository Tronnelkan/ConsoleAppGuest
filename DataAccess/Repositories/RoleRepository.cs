// DataAccess/Repositories/RoleRepository.cs
using DataAccess.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context) { }
                
        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task<bool> ExistsAsync(string roleName)
        {
            return await _dbSet.AnyAsync(r => r.RoleName == roleName);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
