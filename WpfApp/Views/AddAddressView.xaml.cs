// WpfApp/Views/AddAddressView.xaml.cs
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class AddAddressView : Window
    {
        public AddAddressView(AddAddressViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
