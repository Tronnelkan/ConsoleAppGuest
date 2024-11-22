// WpfApp/App.xaml.cs
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using WpfApp.DependencyInjection;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "Host=localhost;Port=5433;Database=MICRO_DB;Username=postgres;Password=tatarinn";

            // Удалено добавление логирования
            // services.AddLogging(configure => configure.AddConsole());

            services.AddDataAccessServices(connectionString)
                    .AddBusinessLogicServices()
                    .AddViewModels()
                    .AddViews();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Запуск главного окна через DI
            var mainView = _serviceProvider.GetRequiredService<MainView>();
            mainView.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }
}
