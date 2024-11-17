using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using DataAccess.Interfaces;

namespace DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetAllWithRolesAsync()
        {
            return await _dbSet.Include(u => u.Role).ToListAsync();
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            return await _dbSet.Include(u => u.Role)
                               .FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}
