using CourseProject.DAL.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CourseProject.DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(CourseProjectContext context) : base(context)
        {
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.AddressEntity) // Переконайтесь, що тут використовується AddressEntity
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.AddressEntity) // І тут теж
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
