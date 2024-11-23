using Domain.Models;
using System.Threading.Tasks;

namespace CourseProject.DAL.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Address> GetByFullAddressAsync(string fullAddress);
    }
}
