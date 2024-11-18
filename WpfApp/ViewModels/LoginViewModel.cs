using BusinessLogic.Services;
using System.Windows.Input;
using WpfApp.Helpers;

namespace WpfApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin(object parameter) => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);

        private async void ExecuteLogin(object parameter)
        {
            bool isAuthenticated = await _userService.AuthenticateUserAsync(Username, Password);
            if (isAuthenticated)
            {
                // Navigate to the main window or perform actions
            }
            else
            {
                // Show error message
            }
        }
    }
}
