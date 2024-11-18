using System.Windows;
using WpfApp.Views;
using WPFApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using WpfApp.Views;

namespace WPFApp
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            var loginView = new LoginView
            {
                DataContext = _serviceProvider.GetRequiredService<LoginViewModel>()
            };
            var mainWindow = new Window
            {
                Content = loginView,
                Title = "Login"
            };
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            services.AddTransient<LoginViewModel>();
        }
    }
}
