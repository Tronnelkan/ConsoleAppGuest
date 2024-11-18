using BusinessLogic.Interfaces;
using System;
using System.Windows.Input;
using WpfApp.Helpers;

namespace WpfApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private string _login;
        private string _password;
        private string _errorMessage;

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
        }

        private async void ExecuteLogin(object parameter)
        {
            try
            {
                var isAuthenticated = await _userService.AuthenticateUserAsync(Login, Password);
                ErrorMessage = isAuthenticated ? "Login successful!" : "Invalid credentials.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
