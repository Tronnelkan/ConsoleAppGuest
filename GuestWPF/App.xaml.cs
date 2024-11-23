using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using GuestWPF.ViewModels;
using GuestWPF.Views;
using Microsoft.EntityFrameworkCore;
using CourseProject.DAL.Data;
using CourseProject.DAL.Repositories;
using CourseProject.BLL.Services;

namespace GuestWPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Налаштування DbContext для PostgreSQL
            services.AddDbContext<CourseProjectContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5433;Database=MICRO_DB;Username=postgres;Password=tatarinn"));

            // Реєстрація репозиторіїв
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            // Реєстрація сервісів
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAddressService, AddressService>();

            // Реєстрація ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<ForgotPasswordViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<ManageUsersViewModel>();
            services.AddTransient<AddAddressViewModel>();

            // Реєстрація вікон
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegisterWindow>();
            services.AddTransient<ForgotPasswordWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<ManageUsersWindow>();
            services.AddTransient<AddAddressWindow>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.Startup += Application_Startup;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
