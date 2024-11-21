using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using WpfApp.ViewModels;
using WpfApp.Views;
using Microsoft.Win32;

namespace WpfApp
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            var mainView = _serviceProvider.GetRequiredService<MainView>();
            mainView.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainView>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<RegisterView>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginView>();
            services.AddTransient<RecoveryPasswordViewModel>();
            services.AddTransient<RecoveryPasswordView>();
        }
    }
}
