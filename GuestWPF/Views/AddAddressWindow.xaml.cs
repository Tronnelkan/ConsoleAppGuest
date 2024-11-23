using System.Windows;
using GuestWPF.ViewModels;
using Domain.Models;

namespace GuestWPF.Views
{
    public partial class AddAddressWindow : Window
    {
        public Address NewAddress { get; private set; }

        public AddAddressWindow()
        {
            InitializeComponent();
            var viewModel = new AddAddressViewModel();
            viewModel.RequestClose += ViewModel_RequestClose;
            DataContext = viewModel;
        }

        private void ViewModel_RequestClose(object sender, Address e)
        {
            NewAddress = e;
            DialogResult = true;
            Close();
        }
    }
}
