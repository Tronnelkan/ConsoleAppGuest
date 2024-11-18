using System.Windows;
using System.Windows.Input;
using WpfApp.Helpers;

namespace WpfApp.ViewModels
{
    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object parameter)
        {
            if (Login == "admin" && Password == "admin")
            {
                MessageBox.Show("Вхід успішний!");
                // Тут можна перемикнути вікно або надати доступ
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль!");
            }
        }
    }
}
