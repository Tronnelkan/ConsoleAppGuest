// WpfApp/ViewModels/RecoveryPasswordViewModel.cs
using BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class RecoveryPasswordViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public ICommand RecoverPasswordCommand { get; }

        private string _email;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        private string _recoveryKeyword;
        [Required(ErrorMessage = "Recovery Keyword is required.")]
        public string RecoveryKeyword
        {
            get => _recoveryKeyword;
            set
            {
                if (SetProperty(ref _recoveryKeyword, value))
                {
                    OnPropertyChanged(nameof(RecoveryKeyword));
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
                    OnPropertyChanged(nameof(NewPassword));
                    OnPropertyChanged(nameof(ConfirmNewPassword));
                }
            }
        }

        // Confirm New Password
        private string _confirmNewPassword;
        [Required(ErrorMessage = "Confirm New Password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set
            {
                if (SetProperty(ref _confirmNewPassword, value))
                {
                    OnPropertyChanged(nameof(ConfirmNewPassword));
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
            RecoverPasswordCommand = new RelayCommand(async o => await RecoverPasswordAsync(o), o => !HasErrors);
        }

        private async Task RecoverPasswordAsync(object parameter)
        {
            try
            {
                // Проверка наличия ошибок валидации
                ValidateAllProperties();

                if (HasErrors)
                {
                    var errors = GetAllErrors();
                    ErrorMessage = string.Join("\n", errors);
                    MessageBox.Show($"Please fix the following errors:\n{ErrorMessage}", "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Логика восстановления пароля
                bool isSuccess = await _userService.RecoverPasswordAsync(Email, RecoveryKeyword, NewPassword);

                if (isSuccess)
                {
                    MessageBox.Show("Password recovered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Закрыть окно восстановления пароля
                    var recoveryPasswordWindow = Application.Current.Windows.OfType<RecoveryPasswordView>().FirstOrDefault();
                    recoveryPasswordWindow?.Close();
                }
                else
                {
                    MessageBox.Show("Failed to recover password. Please check your email and recovery keyword.", "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error recovering password: {ex.Message}";
                MessageBox.Show($"Error recovering password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Получает все ошибки валидации для текущей модели.
        /// </summary>
        /// <returns>Список сообщений об ошибках.</returns>
        private IEnumerable<string> GetAllErrors()
        {
            var errors = new List<string>();

            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var error = this[property.Name];
                if (!string.IsNullOrEmpty(error))
                {
                    errors.Add(error);
                }
            }

            return errors;
        }
    }
}
