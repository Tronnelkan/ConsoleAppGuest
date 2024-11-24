using AutoMapper;
using CourseProject.Web.Models;
using Domain.Models;

namespace CourseProject.Web.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Налаштуйте мапінг між сутностями та моделями
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<Role, RoleViewModel>().ReverseMap();
            CreateMap<Address, AddressViewModel>().ReverseMap();
        }
    }
}
