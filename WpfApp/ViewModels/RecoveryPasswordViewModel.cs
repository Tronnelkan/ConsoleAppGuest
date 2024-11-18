using System;
using System.Windows.Input;
using BusinessLogic.Interfaces;
using WpfApp.Helpers;

namespace WpfApp.ViewModels
{
    public class RecoveryPasswordViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        private string _username;
        private string _recoveryKeyword;
        private string _newPassword;
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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand ResetPasswordCommand { get; }

        public RecoveryPasswordViewModel(IUserService userService)
        {
            _userService = userService;
            ResetPasswordCommand = new RelayCommand(ExecuteResetPassword, CanExecuteResetPassword);
        }

        private bool CanExecuteResetPassword(object parameter)
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(RecoveryKeyword) &&
                   !string.IsNullOrEmpty(NewPassword);
        }

        private void ExecuteResetPassword(object parameter)
        {
            try
            {
                _userService.ResetPassword(Username, RecoveryKeyword, NewPassword);
                ErrorMessage = "Password reset successful!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
