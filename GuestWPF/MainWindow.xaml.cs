using System.Windows;
using GuestWPF.ViewModels;

namespace GuestWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
