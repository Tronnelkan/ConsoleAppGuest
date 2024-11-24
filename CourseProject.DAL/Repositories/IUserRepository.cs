using Domain.Models;
using System.Threading.Tasks;

namespace CourseProject.DAL.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByLoginAsync(string login);
        Task<User> GetByEmailAsync(string email);
        Task UpdateUserAsync(User user);

    }
}
