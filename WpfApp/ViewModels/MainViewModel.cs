using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using WpfApp.Helpers;
using WpfApp.Views;
using BusinessLogic.Interfaces;

namespace WpfApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public ICommand OpenRegisterCommand { get; }
        public ICommand OpenLoginCommand { get; }
        public ICommand OpenRecoveryCommand { get; }

        public string ErrorMessage { get; set; }

        // Передаємо IUserService через конструктор
        public MainViewModel(IUserService userService)
        {
            _userService = userService;

            OpenRegisterCommand = new RelayCommand(OpenRegister);
            OpenLoginCommand = new RelayCommand(OpenLogin);
            OpenRecoveryCommand = new RelayCommand(OpenRecovery);
        }

        private void OpenRegister(object parameter)
        {
            var registerView = new RegisterView(new RegisterViewModel(_userService));
            registerView.Show();
        }

        private void OpenLogin(object parameter)
        {
            var loginView = new LoginView(new LoginViewModel(_userService));
            loginView.Show();
        }

        private void OpenRecovery(object parameter)
        {
            var recoveryView = new RecoveryPasswordView(new RecoveryPasswordViewModel(_userService));
            recoveryView.Show();
        }
    }
}
