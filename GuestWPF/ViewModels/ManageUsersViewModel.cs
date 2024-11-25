using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GuestWPF.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CourseProject.BLL.Services;
using Domain.Models;
using System.Windows;

namespace GuestWPF.ViewModels
{
    public class ManageUsersViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                ((RelayCommand)SaveUserCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand SaveUserCommand { get; }
        public ICommand LoadUsersCommand { get; }

        public ManageUsersViewModel(IUserService userService)
        {
            _userService = userService;
            SaveUserCommand = new RelayCommand(async _ => await SaveUserAsync(), _ => CanSaveUser());
            LoadUsersCommand = new RelayCommand(async _ => await LoadUsersAsync(), _ => true);
            _ = LoadUsersAsync(); // Завантажити користувачів при ініціалізації
        }

        private bool CanSaveUser()
        {
            return SelectedUser != null && !string.IsNullOrWhiteSpace(SelectedUser.Email);
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();

            foreach (var user in users)
            {
                if (user == null)
                {
                    Console.WriteLine("User is null.");
                    continue;
                }

                Console.WriteLine($"User: {user.Login}, AddressId: {user.AddressId}, RoleId: {user.RoleId}");

                if (user.AddressEntity == null && user.AddressId > 0)
                {
                    try
                    {
                        var address = await _addressService.GetAddressByIdAsync(user.AddressId);
                        if (address != null)
                        {
                            user.AddressEntity = address;
                        }
                        else
                        {
                            Console.WriteLine($"Address with ID {user.AddressId} not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while fetching address with ID {user.AddressId}: {ex.Message}");
                    }
                }
            }

            Users = new ObservableCollection<User>(users);
        }






        private async Task SaveUserAsync()
        {
            if (SelectedUser != null)
            {
                try
                {
                    await _userService.UpdateUserAsync(SelectedUser);
                    // Можливо, додати повідомлення про успішне збереження
                }
                catch (Exception ex)
                {
                    // Обробка помилок
                    MessageBox.Show($"Помилка при збереженні користувача: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
