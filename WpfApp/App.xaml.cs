using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            base.OnStartup(e);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<LoginViewModel>();
        }
    }
}
