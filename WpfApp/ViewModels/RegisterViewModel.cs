using System.Windows.Input;
using BusinessLogic.Interfaces;
using Domain.Models;
using WpfApp.Helpers;

namespace WpfApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;
            RegisterCommand = new RelayCommand(Register);
        }

        private void Register(object parameter)
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match!";
                return;
            }

            try
            {
                var newUser = new User
                {
                    Login = Username,
                    PasswordHash = Password,
                    Email = Email
                };

                _userService.RegisterUserAsync(newUser);
                ErrorMessage = "Registration successful!";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
