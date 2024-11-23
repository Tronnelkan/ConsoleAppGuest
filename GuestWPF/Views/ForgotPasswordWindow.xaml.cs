using System.Windows;
using System.Windows.Controls;
using GuestWPF.ViewModels;

namespace GuestWPF.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow(ForgotPasswordViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ForgotPasswordViewModel viewModel)
            {
                viewModel.NewPassword = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ForgotPasswordViewModel viewModel)
            {
                viewModel.ConfirmNewPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
