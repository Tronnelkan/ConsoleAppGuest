using System.Windows;
using GuestWPF.ViewModels;

namespace GuestWPF.Views
{
    public partial class ManageUsersWindow : Window
    {
        public ManageUsersWindow(ManageUsersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
