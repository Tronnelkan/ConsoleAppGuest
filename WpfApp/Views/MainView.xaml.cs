// WpfApp/Views/MainView.xaml.cs
using System.Windows;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp.Views
{
    public partial class MainView : Window
    {
        private readonly RegisterViewModel _registerViewModel;
        private readonly RecoveryPasswordViewModel _recoveryPasswordViewModel;

        public MainView(RegisterViewModel registerViewModel, RecoveryPasswordViewModel recoveryPasswordViewModel)
        {
            InitializeComponent();
            _registerViewModel = registerViewModel;
            _recoveryPasswordViewModel = recoveryPasswordViewModel;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerView = new RegisterView(_registerViewModel);
            registerView.Owner = this;
            registerView.ShowDialog();
        }

        private void RecoverPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var recoveryView = new RecoveryPasswordView(_recoveryPasswordViewModel);
            recoveryView.Owner = this;
            recoveryView.ShowDialog();
        }
    }
}
