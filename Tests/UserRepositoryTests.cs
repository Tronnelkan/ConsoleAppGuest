using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Models;
using DataAccess;
using DataAccess.Repositories;
using System.Linq;

namespace Tests
{
    public class UserRepositoryTests
    {
        private async Task<AppDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_User")
                .Options;
            var context = new AppDbContext(options);

            if (!context.Roles.Any())
            {
                context.Roles.Add(new Role { RoleId = 1, RoleName = "Guest" });
                context.Roles.Add(new Role { RoleId = 2, RoleName = "Admin" });
            }

            if (!context.Addresses.Any())
            {
                context.Addresses.Add(new Address { AddressId = 1, Street = "Test St", City = "Test City", Country = "Test Country" });
            }

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task CreateUser_Test()
        {
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Login = "testuser",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                RecoveryKeyword = "keyword",
                Gender = "Other",
                Email = "test@example.com",
                Phone = "1234567890",
                BankCardData = "0000-1111-2222-3333",
                RoleId = 1,
                AddressId = 1
            };

            await repository.CreateAsync(user);
            var retrievedUser = await repository.GetByIdAsync(user.UserId);

            Assert.NotNull(retrievedUser);
            Assert.Equal("testuser", retrievedUser.Login);
        }

        [Fact]
        public async Task GetAllUsers_Test()
        {
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            context.Users.Add(new User
            {
                Login = "user1",
                FirstName = "User",
                LastName = "One",
                PasswordHash = "hashedpassword1",
                RecoveryKeyword = "keyword1",
                Gender = "Male",
                Email = "user1@example.com",
                Phone = "1111111111",
                BankCardData = "1111-2222-3333-4444",
                RoleId = 1,
                AddressId = 1
            });
            context.Users.Add(new User
            {
                Login = "user2",
                FirstName = "User",
                LastName = "Two",
                PasswordHash = "hashedpassword2",
                RecoveryKeyword = "keyword2",
                Gender = "Female",
                Email = "user2@example.com",
                Phone = "2222222222",
                BankCardData = "5555-6666-7777-8888",
                RoleId = 1,
                AddressId = 1
            });
            await context.SaveChangesAsync();

            var users = await repository.GetAllAsync();

            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task UpdateUser_Test()
        {
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Login = "updateuser",
                FirstName = "Update",
                LastName = "User",
                PasswordHash = "hashedpassword",
                RecoveryKeyword = "keyword",
                Gender = "Male",
                Email = "update@example.com",
                Phone = "3333333333",
                BankCardData = "9999-0000-1111-2222",
                RoleId = 1,
                AddressId = 1
            };
            await repository.CreateAsync(user);

            user.Email = "updated@example.com";
            await repository.UpdateAsync(user);
            var updatedUser = await repository.GetByIdAsync(user.UserId);

            Assert.Equal("updated@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task DeleteUser_Test()
        {
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Login = "deleteuser",
                FirstName = "Delete",
                LastName = "User",
                PasswordHash = "hashedpassword",
                RecoveryKeyword = "keyword",
                Gender = "Female",
                Email = "delete@example.com",
                Phone = "4444444444",
                BankCardData = "3333-4444-5555-6666",
                RoleId = 1,
                AddressId = 1
            };
            await repository.CreateAsync(user);

            await repository.DeleteAsync(user.UserId);
            var deletedUser = await repository.GetByIdAsync(user.UserId);

            Assert.Null(deletedUser);
        }
    }
}
