using CourseProject.BLL.Services;
using Domain.Models;
using GuestWPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GuestWPF.ViewModels
{
    public class ManageUsersViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public ObservableCollection<User> Users { get; set; }

        public ICommand RefreshCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public ManageUsersViewModel(IUserService userService)
        {
            _userService = userService;
            Users = new ObservableCollection<User>();
            RefreshCommand = new RelayCommand(async _ => await LoadUsers());
            DeleteUserCommand = new RelayCommand(async user => await DeleteUser(user as User), user => user != null);

            // Завантаження користувачів при ініціалізації
            Task.Run(async () => await LoadUsers());
        }

        private async Task LoadUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            });
        }

        private async Task DeleteUser(User user)
        {
            if (user != null)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити користувача '{user.Login}'?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _userService.DeleteUserAsync(user.UserId);
                        Users.Remove(user);
                        MessageBox.Show("Користувача видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Сталася помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
