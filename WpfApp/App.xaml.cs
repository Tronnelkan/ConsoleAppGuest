using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp.ViewModels;

namespace WpfApp
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();

            // Реєструємо ViewModels та сервіси
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<Views.MainWindow>();

            // Підключення бізнес-логіки
            services.AddScoped<BusinessLogic.SomeService>(); // Замініть на ваш сервіс
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<Views.MainWindow>();
            mainWindow?.Show();
            base.OnStartup(e);
        }
    }
}
