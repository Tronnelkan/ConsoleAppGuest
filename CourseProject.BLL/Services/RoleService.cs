using CourseProject.BLL.Services;
using CourseProject.DAL.Repositories;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _roleRepository.GetByNameAsync(roleName);
        }

        public async Task AddRoleAsync(Role role)
        {
            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role != null)
            {
                _roleRepository.Delete(role);
                await _roleRepository.SaveChangesAsync();
            }
        }
    }
}
