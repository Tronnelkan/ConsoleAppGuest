// WpfApp/ViewModels/LoginViewModel.cs
using BusinessLogic.Interfaces;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISessionService _sessionService;

        public ICommand LoginCommand { get; }

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

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public LoginViewModel(IUserService userService, IServiceProvider serviceProvider, ISessionService sessionService)
        {
            _userService = userService;
            _serviceProvider = serviceProvider;
            _sessionService = sessionService;
            LoginCommand = new RelayCommand(async o => await LoginAsync(o), o => CanLogin());
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private async Task LoginAsync(object parameter)
        {
            try
            {
                // Отладочное сообщение для проверки выполнения команды
                MessageBox.Show("LoginCommand выполнен");

                User user = await _userService.AuthenticateUserAsync(Username, Password);
                bool isAuthenticated = user != null;

                if (isAuthenticated)
                {
                    _sessionService.CurrentUser = user;
                    var mainView = _serviceProvider.GetRequiredService<MainView>();
                    mainView.Show();
                    // Закрытие окна логина
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this)?.Close();
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
