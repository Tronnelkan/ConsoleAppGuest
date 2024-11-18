using System;
using System.Windows.Input;
using WPFApp.Commands;
using WPFApp.Helpers;

namespace WPFApp.ViewModels
{
    public class RecoverPasswordViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private string _username;
        private string _recoveryKeyword;
        private string _newPassword;
        private string _confirmPassword;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string RecoveryKeyword
        {
            get => _recoveryKeyword;
            set => SetProperty(ref _recoveryKeyword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RecoverPasswordCommand { get; }

        public RecoverPasswordViewModel(IUserService userService)
        {
            _userService = userService;
            RecoverPasswordCommand = new RelayCommand(ExecuteRecoverPassword, CanExecuteRecoverPassword);
        }

        private bool CanExecuteRecoverPassword(object parameter)
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(RecoveryKeyword) &&
                   !string.IsNullOrEmpty(NewPassword) &&
                   NewPassword == ConfirmPassword;
        }

        private void ExecuteRecoverPassword(object parameter)
        {
            try
            {
                _userService.RecoverPassword(Username, RecoveryKeyword, NewPassword);
                ErrorMessage = "Password successfully recovered!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
