// WpfApp/DependencyInjection/ServiceCollectionExtensions.cs
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ISessionService, SessionService>();
            // Добавьте другие сервисы бизнес-логики при необходимости
            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<MainViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RecoveryPasswordViewModel>();
            services.AddTransient<AddAddressViewModel>(); // Добавлено
            return services;
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddTransient<MainView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<LoginView>();
            services.AddTransient<RecoveryPasswordView>();
            services.AddTransient<AddAddressView>(); // Добавлено
            return services;
        }
    }
}
