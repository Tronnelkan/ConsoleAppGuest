using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CourseProject.BLL.Services;
using Domain.Models;
using GuestWPF.Commands;
using GuestWPF.Helpers;
using GuestWPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GuestWPF.ViewModels
{
    public class RegisterViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAddressService _addressService;

        // Поля для введення користувача
        private string _login;
        private string _firstName;
        private string _lastName;
        private string _password;
        private string _confirmPassword;
        private string _recoveryKeyword;
        private string _gender;
        private string _email;
        private string _phone;
        private string _bankCardData;

        // Колекції для ComboBox
        public ObservableCollection<Role> Roles { get; set; }
        public ObservableCollection<Address> Addresses { get; set; }
        public ObservableCollection<string> Genders { get; set; }

        // Вибрані значення
        private Role _selectedRole;
        private Address _selectedAddress;

        public Role SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); ValidateProperty(value, nameof(SelectedRole)); }
        }

        public Address SelectedAddress
        {
            get => _selectedAddress;
            set { _selectedAddress = value; OnPropertyChanged(); ValidateProperty(value, nameof(SelectedAddress)); }
        }

        // Команди
        public ICommand RegisterCommand { get; }
        public ICommand AddRoleCommand { get; }
        public ICommand AddAddressCommand { get; }

        public RegisterViewModel(IUserService userService, IRoleService roleService, IAddressService addressService)
        {
            _userService = userService;
            _roleService = roleService;
            _addressService = addressService;

            Roles = new ObservableCollection<Role>();
            Addresses = new ObservableCollection<Address>();
            Genders = new ObservableCollection<string> { "Чоловіча", "Жіноча", "Інша" };

            RegisterCommand = new RelayCommand(async _ => await Register(), _ => CanRegister());
            AddRoleCommand = new RelayCommand(async _ => await AddRole());
            AddAddressCommand = new RelayCommand(async _ => await AddAddress());

            // Завантаження даних
            Task.Run(async () => await LoadRolesAndAddresses());
        }

        private async Task LoadRolesAndAddresses()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var addresses = await _addressService.GetAllAddressesAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Roles.Clear();
                foreach (var role in roles)
                    Roles.Add(role);

                Addresses.Clear();
                foreach (var address in addresses)
                    Addresses.Add(address);
            });
        }

        private async Task Register()
        {
            try
            {
                // Перевірка наявних помилок
                if (HasErrors)
                {
                    MessageBox.Show("Виправте помилки перед реєстрацією.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Створення користувача
                var user = new User
                {
                    Login = this.Login,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Email = this.Email,
                    Gender = this.Gender,
                    Phone = this.Phone,
                    BankCardData = this.BankCardData,
                    RoleId = SelectedRole.RoleId,
                    AddressId = SelectedAddress.AddressId,
                    RecoveryKeyword = this.RecoveryKeyword // Використовується введене Recovery Keyword
                };

                var registeredUser = await _userService.RegisterUserAsync(user, this.Password);

                MessageBox.Show("Реєстрація успішна!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                // Перехід до вікна входу
                var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is RegisterWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка: {ex.Message}\nДеталі: {ex.InnerException?.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Login) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(RecoveryKeyword) &&
                   SelectedRole != null &&
                   SelectedAddress != null &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(BankCardData) &&
                   !HasErrors;
        }


        private async Task AddRole()
        {
            // Відкрити діалог для додавання нової ролі
            var newRoleName = Prompt.ShowDialog("Введіть назву нової ролі:", "Додати Роль");
            if (!string.IsNullOrWhiteSpace(newRoleName))
            {
                var newRole = new Role { RoleName = newRoleName };
                await _roleService.AddRoleAsync(newRole);
                Roles.Add(newRole);
                SelectedRole = newRole;
            }
        }

        private async Task AddAddress()
        {
            // Відкрити вікно для додавання нової адреси
            var addAddressWindow = new AddAddressWindow();
            if (addAddressWindow.ShowDialog() == true)
            {
                var newAddress = addAddressWindow.NewAddress;
                if (newAddress != null)
                {
                    await _addressService.AddAddressAsync(newAddress);
                    Addresses.Add(newAddress);
                    SelectedAddress = newAddress;
                }
            }
        }


        // Властивості для полів форми
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); ValidateProperty(value, nameof(Login)); }
        }

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); ValidateProperty(value, nameof(FirstName)); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); ValidateProperty(value, nameof(LastName)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); ValidateProperty(value, nameof(Password)); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); ValidateProperty(value, nameof(ConfirmPassword)); }
        }

        public string RecoveryKeyword
        {
            get => _recoveryKeyword;
            set { _recoveryKeyword = value; OnPropertyChanged(); ValidateProperty(value, nameof(RecoveryKeyword)); }
        }

        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); ValidateProperty(value, nameof(Gender)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); ValidateProperty(value, nameof(Email)); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); ValidateProperty(value, nameof(Phone)); }
        }

        public string BankCardData
        {
            get => _bankCardData;
            set { _bankCardData = value; OnPropertyChanged(); ValidateProperty(value, nameof(BankCardData)); }
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
                case nameof(Login):
                    if (string.IsNullOrWhiteSpace(Login))
                    {
                        results.Add("Логін не може бути порожнім.");
                    }
                    // Додайте додаткові перевірки логіну, якщо необхідно
                    break;

                case nameof(FirstName):
                    if (string.IsNullOrWhiteSpace(FirstName))
                    {
                        results.Add("Ім'я не може бути порожнім.");
                    }
                    break;

                case nameof(LastName):
                    if (string.IsNullOrWhiteSpace(LastName))
                    {
                        results.Add("Прізвище не може бути порожнім.");
                    }
                    break;

                case nameof(Password):
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        results.Add("Пароль не може бути порожнім.");
                    }
                    else if (Password.Length < 6)
                    {
                        results.Add("Пароль повинен містити щонайменше 6 символів.");
                    }
                    break;

                case nameof(ConfirmPassword):
                    if (string.IsNullOrWhiteSpace(ConfirmPassword))
                    {
                        results.Add("Підтвердження пароля не може бути порожнім.");
                    }
                    else if (ConfirmPassword != Password)
                    {
                        results.Add("Паролі не збігаються.");
                    }
                    break;

                case nameof(RecoveryKeyword):
                    if (string.IsNullOrWhiteSpace(RecoveryKeyword))
                    {
                        results.Add("Recovery Keyword не може бути порожнім.");
                    }
                    // Додайте додаткові перевірки Recovery Keyword, якщо необхідно
                    break;

                case nameof(Gender):
                    if (string.IsNullOrWhiteSpace(Gender))
                    {
                        results.Add("Стать не може бути порожньою.");
                    }
                    break;

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

                case nameof(Phone):
                    var phonePattern = @"^\+?\d{10,15}$";
                    if (string.IsNullOrWhiteSpace(Phone))
                    {
                        results.Add("Телефон не може бути порожнім.");
                    }
                    else if (!Regex.IsMatch(Phone, phonePattern))
                    {
                        results.Add("Невірний формат телефону.");
                    }
                    break;

                case nameof(BankCardData):
                    var cardPattern = @"^\d{16}$"; // Приклад: 16-значний номер картки
                    if (string.IsNullOrWhiteSpace(BankCardData))
                    {
                        results.Add("Дані банківської картки не можуть бути порожніми.");
                    }
                    else if (!Regex.IsMatch(BankCardData, cardPattern))
                    {
                        results.Add("Невірний формат даних банківської картки.");
                    }
                    break;

                case nameof(SelectedRole):
                    if (SelectedRole == null)
                    {
                        results.Add("Виберіть роль.");
                    }
                    break;

                case nameof(SelectedAddress):
                    if (SelectedAddress == null)
                    {
                        results.Add("Виберіть адресу.");
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
