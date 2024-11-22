// WpfApp/App.xaml.cs
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            services.AddDataAccessServices(connectionString)
                    .AddBusinessLogicServices()
                    .AddViewModels()
                    .AddViews();
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            // Запуск главного меню
            var mainView = _serviceProvider.GetRequiredService<MainView>();
            mainView.Show();

            base.OnStartup(e);
        }
    }
}
