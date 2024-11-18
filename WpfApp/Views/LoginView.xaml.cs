using System.Windows;
using WpfApp.ViewModels;
using WPFApp.ViewModels;

namespace WpfApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
