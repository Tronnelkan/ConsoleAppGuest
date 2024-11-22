// WpfApp/ViewModels/AddAddressViewModel.cs
using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using System.Linq;

namespace WpfApp.ViewModels
{
    public class AddAddressViewModel : BaseViewModel
    {
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private string _street;
        [Required(ErrorMessage = "Street is required.")]
        [MaxLength(150)]
        public string Street
        {
            get => _street;
            set
            {
                if (SetProperty(ref _street, value))
                {
                    ValidateProperty(value);
                }
            }
        }

        private string _city;
        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100)]
        public string City
        {
            get => _city;
            set
            {
                if (SetProperty(ref _city, value))
                {
                    ValidateProperty(value);
                }
            }
        }

        private string _country;
        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(100)]
        public string Country
        {
            get => _country;
            set
            {
                if (SetProperty(ref _country, value))
                {
                    ValidateProperty(value);
                }
            }
        }

        public Address NewAddress { get; private set; }

        public AddAddressViewModel()
        {
            SaveCommand = new RelayCommand(async o => await SaveAsync(), o => !HasErrors);
            CancelCommand = new RelayCommand(async o => { Cancel(); await Task.CompletedTask; }, o => true);

            this.ErrorsChanged += (s, e) =>
            {
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            };
        }

        private async Task SaveAsync()
        {
            try
            {
                ValidateAllProperties();

                if (HasErrors)
                {
                    MessageBox.Show("Please fix the errors before saving.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                NewAddress = new Address
                {
                    Street = this.Street,
                    City = this.City,
                    Country = this.Country
                };

                // Закрытие окна с DialogResult = true
                var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            // Закрытие окна без сохранения
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
