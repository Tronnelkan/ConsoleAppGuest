// WpfApp/ViewModels/RegisterViewModel.cs
using BusinessLogic.Interfaces;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

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
                    ValidateProperty(value);
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
                    ValidateProperty(value);
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
                    ValidateProperty(value);
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
                    ValidateProperty(value);
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
                    ValidateProperty(value);
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
                    ValidateProperty(value);
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
                    ValidateProperty(value);
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
                    ValidateProperty(value);
                    ValidateProperty(ConfirmPassword, nameof(ConfirmPassword));
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
                    ValidateProperty(value);
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

        public RegisterViewModel(IUserService userService, IServiceProvider serviceProvider)
        {
            _userService = userService;
            _serviceProvider = serviceProvider;
            RegisterCommand = new RelayCommand(async o => await RegisterAsync(), o => !HasErrors);
            AddAddressCommand = new RelayCommand(async o => await AddAddressAsync(), o => true);

            // Загрузка ролей и адресов после инициализации ViewModel
            Loaded += async () => await LoadRolesAndAddressesAsync();

            // Обновление CanExecute при изменении ошибок
            this.ErrorsChanged += (s, e) =>
            {
                (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (AddAddressCommand as RelayCommand)?.RaiseCanExecuteChanged();
            };
        }

        // Событие, которое вызывается при загрузке ViewModel
        public event Func<Task> Loaded;

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
                Console.WriteLine($"Ошибка при загрузке ролей или адресов: {ex}");
            }
        }

        private async Task RegisterAsync()
        {
            try
            {
                ValidateAllProperties();
                Console.WriteLine("Валидация завершена. HasErrors: " + HasErrors);
                Console.WriteLine($"Password: {Password}");
                Console.WriteLine($"ConfirmPassword: {ConfirmPassword}");

                if (HasErrors)
                {
                    var errors = GetErrors(null).Cast<string>();
                    ErrorMessage = string.Join("\n", errors);
                    Console.WriteLine("Ошибки валидации: " + ErrorMessage);
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

                Console.WriteLine("Создание пользователя: " + user.Login);

                await _userService.RegisterUserAsync(user, Password);
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Console.WriteLine("Пользователь успешно зарегистрирован.");

                // Закрыть окно регистрации
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this)?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error registering user: {ex.Message}";
                Console.WriteLine($"Исключение при регистрации: {ex}");
                MessageBox.Show($"Error registering user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddAddressAsync()
        {
            try
            {
                // Получаем экземпляр AddAddressView через DI
                var addAddressView = _serviceProvider.GetRequiredService<AddAddressView>();
                addAddressView.Owner = Application.Current.MainWindow;
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
                Console.WriteLine($"Ошибка при добавлении адреса: {ex}");
                MessageBox.Show($"Error adding address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
