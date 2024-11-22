// WpfApp/ViewModels/MainViewModel.cs
using BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;

        public ICommand OpenRegisterCommand { get; }
        public ICommand OpenLoginCommand { get; }
        public ICommand OpenRecoveryPasswordCommand { get; }

        public ICommand LogoutCommand { get; }

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            OpenRegisterCommand = new RelayCommand(async o => await OpenRegisterAsync());
            OpenLoginCommand = new RelayCommand(async o => await OpenLoginAsync());
            OpenRecoveryPasswordCommand = new RelayCommand(async o => await OpenRecoveryPasswordAsync());

            LogoutCommand = new RelayCommand(async o => await LogoutAsync());
        }

        private async Task OpenRegisterAsync()
        {
            var registerView = _serviceProvider.GetRequiredService<RegisterView>();
            registerView.Show();
            await Task.CompletedTask;
        }

        private async Task OpenLoginAsync()
        {
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();
            await Task.CompletedTask;
        }

        private async Task OpenRecoveryPasswordAsync()
        {
            var recoveryView = _serviceProvider.GetRequiredService<RecoveryPasswordView>();
            recoveryView.Show();
            await Task.CompletedTask;
        }

        private async Task LogoutAsync()
        {
            // Логика выхода из системы (например, очистка сессии)
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();

            // Закрытие текущего окна
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this)?.Close();
            await Task.CompletedTask;
        }
    }
}
