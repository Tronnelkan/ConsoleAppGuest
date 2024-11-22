// WpfApp/ViewModels/LoginViewModel.cs
using BusinessLogic.Interfaces;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoginViewModel> _logger;
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

        public LoginViewModel(IUserService userService, IServiceProvider serviceProvider, ILogger<LoginViewModel> logger, ISessionService sessionService)
        {
            _userService = userService;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _sessionService = sessionService;
            LoginCommand = new RelayCommand(async o => await LoginAsync(), o => CanLogin());
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private async Task LoginAsync()
        {
            try
            {
                _logger.LogInformation("Attempting to authenticate user {Username}", Username);
                User user = await _userService.AuthenticateUserAsync(Username, Password);
                bool isAuthenticated = user != null;

                if (isAuthenticated)
                {
                    _sessionService.CurrentUser = user;
                    _logger.LogInformation("User {Username} authenticated successfully.", Username);
                    var mainView = _serviceProvider.GetRequiredService<MainView>();
                    mainView.Show();
                    // Закрытие окна логина
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this)?.Close();
                }
                else
                {
                    _logger.LogWarning("Authentication failed for user {Username}.", Username);
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user {Username}.", Username);
                ErrorMessage = ex.Message;
            }
        }
    }
}
