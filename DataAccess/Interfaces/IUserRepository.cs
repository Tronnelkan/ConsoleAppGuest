using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByLoginAsync(string login);
        Task<IEnumerable<User>> GetAllWithRolesAsync();
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
