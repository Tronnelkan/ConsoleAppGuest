// DependencyInjection/ServiceCollectionExtensions.cs
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DataAccess;
using Microsoft.Win32;
using System;

namespace DependencyInjection
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
            // Додайте інші сервіси бізнес-логіки за потреби
            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<MainViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RecoveryPasswordViewModel>();
            // Додайте інші ViewModels за потреби
            return services;
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddTransient<MainView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<LoginView>();
            services.AddTransient<RecoveryPasswordView>();
            // Додайте інші Views за потреби
            return services;
        }
    }
}
