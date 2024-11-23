using CourseProject.DAL.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CourseProject.DAL.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(CourseProjectContext context) : base(context)
        {
        }

        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}
