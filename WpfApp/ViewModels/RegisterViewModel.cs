// WpfApp/ViewModels/RegisterViewModel.cs
using BusinessLogic.Interfaces;
using Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace WpfApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;

        public ICommand RegisterCommand { get; }
        public ICommand AddAddressCommand { get; }

        // Username
        private string _username;
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        // Email
        private string _email;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100)]
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

        // First Name
        private string _firstName;
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value))
                {
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        // Last Name
        private string _lastName;
        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value))
                {
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        // Gender
        private string _gender;
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender
        {
            get => _gender;
            set
            {
                if (SetProperty(ref _gender, value))
                {
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }

        private ObservableCollection<string> _genders = new ObservableCollection<string> { "Male", "Female", "Other" };
        public ObservableCollection<string> Genders
        {
            get => _genders;
            set => SetProperty(ref _genders, value);
        }

        // Phone
        private string _phone;
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone
        {
            get => _phone;
            set
            {
                if (SetProperty(ref _phone, value))
                {
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        // Bank Card Data
        private string _bankCardData;
        [Required(ErrorMessage = "Bank card data is required.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Bank card number must be exactly 16 digits.")]
        public string BankCardData
        {
            get => _bankCardData;
            set
            {
                if (SetProperty(ref _bankCardData, value))
                {
                    OnPropertyChanged(nameof(BankCardData));
                }
            }
        }

        // Password
        private string _password;
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    // Trigger validation for ConfirmPassword as well
                    OnPropertyChanged(nameof(Password));
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }

        // Confirm Password
        private string _confirmPassword;
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                {
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }

        // Recovery Keyword
        private string _recoveryKeyword;
        [Required(ErrorMessage = "Recovery Keyword is required.")]
        public string RecoveryKeyword
        {
            get => _recoveryKeyword;
            set => SetProperty(ref _recoveryKeyword, value);
        }

        // Roles
        private ObservableCollection<Role> _roles;
        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        private Role _selectedRole;
        [Required(ErrorMessage = "Please select a role.")]
        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    OnPropertyChanged(nameof(SelectedRole));
                }
            }
        }

        // Addresses
        private ObservableCollection<Address> _addresses;
        public ObservableCollection<Address> Addresses
        {
            get => _addresses;
            set => SetProperty(ref _addresses, value);
        }

        private Address _selectedAddress;
        [Required(ErrorMessage = "Please select an address.")]
        public Address SelectedAddress
        {
            get => _selectedAddress;
            set
            {
                if (SetProperty(ref _selectedAddress, value))
                {
                    OnPropertyChanged(nameof(SelectedAddress));
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

        public RegisterViewModel(IUserService userService, IServiceProvider serviceProvider)
        {
            _userService = userService;
            _serviceProvider = serviceProvider;

            RegisterCommand = new RelayCommand(async o => await RegisterAsync(o), o => !HasErrors);
            AddAddressCommand = new RelayCommand(async o => await AddAddressAsync(o), o => true);

            // Инициализация данных
            Task.Run(async () => await LoadRolesAndAddressesAsync());
        }

        public async Task LoadRolesAndAddressesAsync()
        {
            try
            {
                var roles = await _userService.GetAllRolesAsync();
                Roles = new ObservableCollection<Role>(roles);

                var addresses = await _userService.GetAllAddressesAsync();
                Addresses = new ObservableCollection<Address>(addresses);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading roles or addresses: {ex.Message}";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error loading roles or addresses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task RegisterAsync(object parameter)
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

                var user = new User
                {
                    Login = this.Username,
                    Email = this.Email,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Gender = this.Gender,
                    Phone = this.Phone,
                    BankCardData = this.BankCardData,
                    RoleId = SelectedRole.RoleId,
                    AddressId = SelectedAddress.AddressId,
                    RecoveryKeyword = this.RecoveryKeyword
                };

                await _userService.RegisterUserAsync(user, Password);
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Закрыть окно регистрации
                var registerWindow = Application.Current.Windows.OfType<RegisterView>().FirstOrDefault();
                registerWindow?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error registering user: {ex.Message}";
                MessageBox.Show($"Error registering user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddAddressAsync(object parameter)
        {
            try
            {
                // Получаем экземпляр AddAddressView через DI
                var addAddressView = _serviceProvider.GetRequiredService<AddAddressView>();
                addAddressView.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault();
                bool? result = addAddressView.ShowDialog();

                if (result == true)
                {
                    // Получаем ViewModel через DataContext
                    if (addAddressView.DataContext is AddAddressViewModel addAddressViewModel)
                    {
                        var newAddress = addAddressViewModel.NewAddress;
                        if (newAddress != null)
                        {
                            await _userService.AddAddressAsync(newAddress);
                            Addresses.Add(newAddress);
                            SelectedAddress = newAddress;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding address: {ex.Message}";
                MessageBox.Show($"Error adding address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
