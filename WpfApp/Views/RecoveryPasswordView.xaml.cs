// WpfApp/Views/RecoveryPasswordView.xaml.cs
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class RecoveryPasswordView : Window
    {
        public RecoveryPasswordView(RecoveryPasswordViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
