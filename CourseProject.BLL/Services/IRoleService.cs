using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task AddRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(int id);
    }
}
