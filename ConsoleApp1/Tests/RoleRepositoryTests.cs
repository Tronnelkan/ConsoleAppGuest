using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Models;
using DataAccess;
using DataAccess.Repositories;
using System.Linq;

namespace Tests
{
    public class RoleRepositoryTests
    {
        private async Task<AppDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Role")
                .Options;
            var context = new AppDbContext(options);

            if (!context.Roles.Any())
            {
                context.Roles.Add(new Role { RoleId = 1, RoleName = "Guest" });
                context.Roles.Add(new Role { RoleId = 2, RoleName = "Admin" });
            }

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task CreateRole_Test()
        {
            var context = await GetDbContext();
            var repository = new RoleRepository(context);
            var role = new Role
            {
                RoleName = "Tester"
            };

            await repository.CreateAsync(role);
            var retrievedRole = await repository.GetByIdAsync(role.RoleId);

            Assert.NotNull(retrievedRole);
            Assert.Equal("Tester", retrievedRole.RoleName);
        }

        [Fact]
        public async Task GetAllRoles_Test()
        {
            var context = await GetDbContext();
            var repository = new RoleRepository(context);

            var roles = await repository.GetAllAsync();

            Assert.Equal(2, roles.Count());
        }

        [Fact]
        public async Task UpdateRole_Test()
        {
            var context = await GetDbContext();
            var repository = new RoleRepository(context);
            var role = new Role
            {
                RoleName = "Updater"
            };
            await repository.CreateAsync(role);

            role.RoleName = "UpdatedRole";
            await repository.UpdateAsync(role);
            var updatedRole = await repository.GetByIdAsync(role.RoleId);

            Assert.Equal("UpdatedRole", updatedRole.RoleName);
        }

        [Fact]
        public async Task DeleteRole_Test()
        {
            var context = await GetDbContext();
            var repository = new RoleRepository(context);
            var role = new Role
            {
                RoleName = "Deleter"
            };
            await repository.CreateAsync(role);

            await repository.DeleteAsync(role.RoleId);
            var deletedRole = await repository.GetByIdAsync(role.RoleId);

            Assert.Null(deletedRole);
        }
    }
}
