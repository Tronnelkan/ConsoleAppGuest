// WpfApp/ViewModels/RecoveryPasswordViewModel.cs
using BusinessLogic.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    public class RecoveryPasswordViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public ICommand RecoverPasswordCommand { get; }

        // Username
        private string _username;
        [Required(ErrorMessage = "Username is required.")]
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ValidateProperty(value);
                }
            }
        }

        // Recovery Keyword
        private string _recoveryKeyword;
        [Required(ErrorMessage = "Recovery Keyword is required.")]
        public string RecoveryKeyword
        {
            get => _recoveryKeyword;
            set
            {
                if (SetProperty(ref _recoveryKeyword, value))
                {
                    ValidateProperty(value);
                }
            }
        }

        // New Password
        private string _newPassword;
        [Required(ErrorMessage = "New Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value))
                {
                    ValidateProperty(value);
                    ValidateProperty(ConfirmPassword, nameof(ConfirmPassword));
                }
            }
        }

        // Confirm Password
        private string _confirmPassword;
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                {
                    ValidateProperty(value);
                }
            }
        }

        // Error Message
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public RecoveryPasswordViewModel(IUserService userService)
        {
            _userService = userService;
            RecoverPasswordCommand = new RelayCommand(async o => await RecoverPasswordAsync(), o => !HasErrors);

            // Обновление CanExecute при изменении ошибок
            this.ErrorsChanged += (s, e) =>
            {
                (RecoverPasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
            };
        }

        private async Task RecoverPasswordAsync()
        {
            try
            {
                ValidateAllProperties();

                if (HasErrors)
                {
                    var errors = GetErrors(null).Cast<string>();
                    ErrorMessage = string.Join("\n", errors);
                    return;
                }

                // Логика восстановления пароля
                bool success = await _userService.RecoverPasswordAsync(Username, RecoveryKeyword, NewPassword);

                if (success)
                {
                    MessageBox.Show("Password successfully recovered!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Закрыть окно восстановления пароля
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this)?.Close();
                }
                else
                {
                    ErrorMessage = "Invalid username or recovery keyword.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error recovering password: {ex.Message}";
            }
        }
    }
}
