using System.Windows;
using System.Windows.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class RecoveryPasswordView : Window
    {
        public RecoveryPasswordView(RecoveryPasswordViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RecoveryPasswordViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.NewPassword = passwordBox.Password;
            }
        }
    }
}
