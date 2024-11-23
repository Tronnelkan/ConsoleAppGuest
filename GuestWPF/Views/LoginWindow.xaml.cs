using System.Windows;
using System.Windows.Controls;
using GuestWPF.ViewModels;

namespace GuestWPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
