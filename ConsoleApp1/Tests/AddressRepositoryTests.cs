using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Models;
using DataAccess;
using DataAccess.Repositories;
using System.Linq;

namespace Tests
{
    public class AddressRepositoryTests
    {
        private async Task<AppDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Address")
                .Options;
            var context = new AppDbContext(options);

            if (!context.Addresses.Any())
            {
                context.Addresses.Add(new Address { AddressId = 1, Street = "Initial St", City = "Initial City", Country = "Initial Country" });
            }

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task CreateAddress_Test()
        {
            var context = await GetDbContext();
            var repository = new AddressRepository(context);
            var address = new Address
            {
                Street = "New St",
                City = "New City",
                Country = "New Country"
            };

            await repository.CreateAsync(address);
            var retrievedAddress = await repository.GetByIdAsync(address.AddressId);

            Assert.NotNull(retrievedAddress);
            Assert.Equal("New St", retrievedAddress.Street);
        }

        [Fact]
        public async Task GetAllAddresses_Test()
        {
            var context = await GetDbContext();
            var repository = new AddressRepository(context);
            context.Addresses.Add(new Address { Street = "Address1", City = "City1", Country = "Country1" });
            context.Addresses.Add(new Address { Street = "Address2", City = "City2", Country = "Country2" });
            await context.SaveChangesAsync();

            var addresses = await repository.GetAllAsync();

            Assert.Equal(2, addresses.Count());
        }

        [Fact]
        public async Task UpdateAddress_Test()
        {
            var context = await GetDbContext();
            var repository = new AddressRepository(context);
            var address = new Address
            {
                Street = "Update St",
                City = "Update City",
                Country = "Update Country"
            };
            await repository.CreateAsync(address);

            address.City = "Updated City";
            await repository.UpdateAsync(address);
            var updatedAddress = await repository.GetByIdAsync(address.AddressId);

            Assert.Equal("Updated City", updatedAddress.City);
        }

        [Fact]
        public async Task DeleteAddress_Test()
        {
            var context = await GetDbContext();
            var repository = new AddressRepository(context);
            var address = new Address
            {
                Street = "Delete St",
                City = "Delete City",
                Country = "Delete Country"
            };
            await repository.CreateAsync(address);

            await repository.DeleteAsync(address.AddressId);
            var deletedAddress = await repository.GetByIdAsync(address.AddressId);

            Assert.Null(deletedAddress);
        }
    }
}
