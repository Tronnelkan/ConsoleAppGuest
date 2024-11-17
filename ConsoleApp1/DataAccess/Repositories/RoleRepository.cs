using System.Threading.Tasks;
using Domain.Models;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task<bool> ExistsAsync(string roleName)
        {
            return await _dbSet.AnyAsync(r => r.RoleName == roleName);
        }
    }
}
