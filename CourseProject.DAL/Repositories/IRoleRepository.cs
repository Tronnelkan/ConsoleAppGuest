using Domain.Models;
using System.Threading.Tasks;

namespace CourseProject.DAL.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetByNameAsync(string roleName);
    }
}
