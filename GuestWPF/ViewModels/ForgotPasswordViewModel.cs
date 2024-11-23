using CourseProject.BLL.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GuestWPF.Commands;
using GuestWPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace GuestWPF.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly IUserService _userService;

        private string _email;
        private string _recoveryKeyword;
        private string _newPassword;
        private string _confirmNewPassword;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                ValidateProperty(value, nameof(Email));
            }
        }

        public string RecoveryKeyword
        {
            get => _recoveryKeyword;
            set
            {
                _recoveryKeyword = value;
                OnPropertyChanged();
                ValidateProperty(value, nameof(RecoveryKeyword));
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
                ValidateProperty(value, nameof(NewPassword));
            }
        }

        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set
            {
                _confirmNewPassword = value;
                OnPropertyChanged();
                ValidateProperty(value, nameof(ConfirmNewPassword));
            }
        }

        public ICommand ResetPasswordCommand { get; }

        public ForgotPasswordViewModel(IUserService userService)
        {
            _userService = userService;
            ResetPasswordCommand = new RelayCommand(async _ => await ResetPassword(), _ => CanResetPassword());
        }

        private bool CanResetPassword()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(RecoveryKeyword) &&
                   !string.IsNullOrWhiteSpace(NewPassword) &&
                   !string.IsNullOrWhiteSpace(ConfirmNewPassword) &&
                   !HasErrors;
        }

        private async Task ResetPassword()
        {
            try
            {
                if (NewPassword != ConfirmNewPassword)
                {
                    MessageBox.Show("Нові паролі не збігаються.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool result = await _userService.ResetPasswordAsync(Email.Trim(), RecoveryKeyword.Trim(), NewPassword);

                if (result)
                {
                    MessageBox.Show("Пароль успішно відновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Відкриття вікна входу через DI
                    var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
                    loginWindow.Show();

                    // Закриття вікна відновлення пароля
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is ForgotPasswordWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Невірний Email або Ключове слово.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show($"Помилка при оновленні даних: {dbEx.InnerException?.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка1: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #region Validation

        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return Enumerable.Empty<string>();

            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];

            return Enumerable.Empty<string>();
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ValidateProperty(object value, string propertyName)
        {
            var results = new List<string>();

            switch (propertyName)
            {
                case nameof(Email):
                    var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                    if (string.IsNullOrWhiteSpace(Email))
                    {
                        results.Add("Email не може бути порожнім.");
                    }
                    else if (!Regex.IsMatch(Email, emailPattern))
                    {
                        results.Add("Невірний формат Email.");
                    }
                    break;

                case nameof(RecoveryKeyword):
                    if (string.IsNullOrWhiteSpace(RecoveryKeyword))
                    {
                        results.Add("Ключове слово не може бути порожнім.");
                    }
                    break;

                case nameof(NewPassword):
                    if (string.IsNullOrWhiteSpace(NewPassword))
                    {
                        results.Add("Новий пароль не може бути порожнім.");
                    }
                    else if (NewPassword.Length < 6)
                    {
                        results.Add("Пароль повинен містити щонайменше 6 символів.");
                    }
                    break;

                case nameof(ConfirmNewPassword):
                    if (string.IsNullOrWhiteSpace(ConfirmNewPassword))
                    {
                        results.Add("Підтвердження пароля не може бути порожнім.");
                    }
                    else if (ConfirmNewPassword != NewPassword)
                    {
                        results.Add("Паролі не збігаються.");
                    }
                    break;
            }

            if (results.Any())
            {
                _errors[propertyName] = results;
            }
            else
            {
                _errors.Remove(propertyName);
            }

            OnErrorsChanged(propertyName);
        }

        #endregion
    }
}
