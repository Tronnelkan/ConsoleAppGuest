using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfApp.ViewModels;
using WPFApp.ViewModels;

namespace WpfApp
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Реєстрація репозиторіїв
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<IAddressRepository, AddressRepository>();

            // Реєстрація сервісів
            services.AddSingleton<IUserService, UserService>();

            // Реєстрація ViewModel
            services.AddTransient<LoginViewModel>();

            // Реєстрація Views
            services.AddTransient<Views.LoginView>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var loginView = _serviceProvider.GetService<Views.LoginView>();
            loginView?.Show();
            base.OnStartup(e);
        }
    }
}
