using System;
using System.Windows.Input;
using BusinessLogic.Services;
using Domain.Models;
using WPFApp.Commands;
using WPFApp.Helpers;

namespace WPFApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private string _username;
        private string _password;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
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

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private void ExecuteLogin(object parameter)
        {
            try
            {
                if (_authService.Authenticate(Username, Password))
                {
                    var role = _authService.GetRole(Username);
                    NavigateToDashboard(role);
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void NavigateToDashboard(string role)
        {
            // TODO: Implement navigation logic to the appropriate dashboard based on the role.
            // Example: if (role == "Admin") -> AdminDashboard
        }
    }
}
