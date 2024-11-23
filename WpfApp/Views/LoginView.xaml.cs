// WpfApp/Views/LoginView.xaml.cs
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
