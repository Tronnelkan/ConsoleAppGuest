using BusinessLogic.Services;
using Domain.Models;
using System;
using System.Windows.Input;
using WpfApp.Helpers;
using WpfApp.ViewModels;

namespace WPFApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _email;
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;
            RegisterCommand = new RelayCommand(ExecuteRegister, CanExecuteRegister);
        }

        private bool CanExecuteRegister(object parameter)
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(Password) &&
                   !string.IsNullOrEmpty(ConfirmPassword) &&
                   Password == ConfirmPassword;
        }

        private void ExecuteRegister(object parameter)
        {
            try
            {
                var user = new User
                {
                    Username = Username,
                    PasswordHash = Password,
                    Email = Email
                };

                _userService.RegisterUser(user);
                ErrorMessage = "Registration successful!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
