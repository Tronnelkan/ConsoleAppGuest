// WpfApp/DependencyInjection/ServiceCollectionExtensions.cs
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Models;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, string connectionString)
        {
            // Настройка DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Регистрация универсальных репозиториев
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Регистрация специфичных репозиториев
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<AddAddressViewModel>();
            services.AddTransient<RecoveryPasswordViewModel>();
            services.AddTransient<BaseViewModel>();

            return services;
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddTransient<MainView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<AddAddressView>();
            services.AddTransient<RecoveryPasswordView>();

            return services;
        }
    }
}
