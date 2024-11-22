// WpfApp/Views/RegisterView.xaml.cs
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView(RegisterViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.ContentRendered += async (s, e) => await viewModel.LoadRolesAndAddressesAsync();
        }
    }
}
