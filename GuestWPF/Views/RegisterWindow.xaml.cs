using System.Windows;
using System.Windows.Controls;
using GuestWPF.ViewModels;

namespace GuestWPF.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow(RegisterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((RegisterViewModel)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((RegisterViewModel)DataContext).ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
