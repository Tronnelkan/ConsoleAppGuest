// WpfApp/ViewModels/MainViewModel.cs
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

            OpenRegisterCommand = new RelayCommand(async o => await OpenRegisterAsync(o), o => true);
            OpenLoginCommand = new RelayCommand(async o => await OpenLoginAsync(o), o => true);
            OpenRecoveryPasswordCommand = new RelayCommand(async o => await OpenRecoveryPasswordAsync(o), o => true);

            LogoutCommand = new RelayCommand(async o => await LogoutAsync(o), o => true);
        }

        private async Task OpenRegisterAsync(object parameter)
        {
            // Отладочное сообщение для проверки выполнения команды
            MessageBox.Show("OpenRegisterCommand выполнен");

            var registerView = _serviceProvider.GetRequiredService<RegisterView>();
            registerView.Show();
            await Task.CompletedTask;
        }

        private async Task OpenLoginAsync(object parameter)
        {
            // Отладочное сообщение для проверки выполнения команды
            MessageBox.Show("OpenLoginCommand выполнен");

            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();
            await Task.CompletedTask;
        }

        private async Task OpenRecoveryPasswordAsync(object parameter)
        {
            // Отладочное сообщение для проверки выполнения команды
            MessageBox.Show("OpenRecoveryPasswordCommand выполнен");

            var recoveryView = _serviceProvider.GetRequiredService<RecoveryPasswordView>();
            recoveryView.Show();
            await Task.CompletedTask;
        }

        private async Task LogoutAsync(object parameter)
        {
            // Отладочное сообщение для проверки выполнения команды
            MessageBox.Show("LogoutCommand выполнен");

            // Логика выхода из системы (например, очистка сессии)
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();

            // Закрытие текущего окна
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this)?.Close();
            await Task.CompletedTask;
        }
    }
}
