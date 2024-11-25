using AutoMapper;
using Domain.Models;
using CourseProject.Web.ViewModels;

namespace CourseProject.Web.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Мапінг RegisterViewModel до User
            CreateMap<RegisterViewModel, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => ComputeHash(src.Password)))
                .ForMember(dest => dest.AddressEntity, opt => opt.Ignore()) // Ігноруємо AddressEntity
                .AfterMap((src, dest) =>
                {
                    // Розділення Address на частини
                    if (!string.IsNullOrWhiteSpace(src.Address))
                    {
                        var parts = src.Address.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 3)
                        {
                            dest.AddressEntity = new Address
                            {
                                Street = parts[0].Trim(),
                                City = parts[1].Trim(),
                                Country = parts[2].Trim()
                            };
                        }
                    }
                });

            // Мапінг User до UserViewModel
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AddressEntity != null ? $"{src.AddressEntity.Street}, {src.AddressEntity.City}, {src.AddressEntity.Country}" : string.Empty))
                .ReverseMap()
                .ForMember(dest => dest.AddressEntity, opt => opt.Ignore()) // Ігноруємо AddressEntity при зворотному мапінгу
                .ForMember(dest => dest.Role, opt => opt.Ignore()); // Ігноруємо Role при зворотному мапінгу

            CreateMap<Address, AddressViewModel>().ReverseMap();
            CreateMap<Role, RoleViewModel>().ReverseMap();
        }

        private string ComputeHash(string password)
        {
            // Ваш метод хешування пароля
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
