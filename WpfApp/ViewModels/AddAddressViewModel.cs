// WpfApp/ViewModels/AddAddressViewModel.cs
using BusinessLogic.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class AddAddressViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public ICommand AddAddressCommand { get; }

        private string _street;
        [Required(ErrorMessage = "Street is required.")]
        public string Street
        {
            get => _street;
            set
            {
                if (SetProperty(ref _street, value))
                {
                    OnPropertyChanged(nameof(Street));
                }
            }
        }

        private string _city;
        [Required(ErrorMessage = "City is required.")]
        public string City
        {
            get => _city;
            set
            {
                if (SetProperty(ref _city, value))
                {
                    OnPropertyChanged(nameof(City));
                }
            }
        }

        private string _country;
        [Required(ErrorMessage = "Country is required.")]
        public string Country
        {
            get => _country;
            set
            {
                if (SetProperty(ref _country, value))
                {
                    OnPropertyChanged(nameof(Country));
                }
            }
        }

        // Error Message
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public Address NewAddress { get; private set; }

        public AddAddressViewModel(IUserService userService)
        {
            _userService = userService;
            AddAddressCommand = new RelayCommand(async o => await AddAddressAsync(o), o => !HasErrors);
        }

        private async Task AddAddressAsync(object parameter)
        {
            try
            {
                // Проверка наличия ошибок валидации
                ValidateAllProperties();

                if (HasErrors)
                {
                    var errors = GetAllErrors();
                    ErrorMessage = string.Join("\n", errors);
                    MessageBox.Show($"Please fix the following errors:\n{ErrorMessage}", "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                NewAddress = new Address
                {
                    Street = this.Street,
                    City = this.City,
                    Country = this.Country
                };

                await _userService.AddAddressAsync(NewAddress);
                MessageBox.Show("Address added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Закрыть окно добавления адреса
                var addAddressWindow = Application.Current.Windows.OfType<AddAddressView>().FirstOrDefault();
                addAddressWindow?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding address: {ex.Message}";
                MessageBox.Show($"Error adding address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Получает все ошибки валидации для текущей модели.
        /// </summary>
        /// <returns>Список сообщений об ошибках.</returns>
        private IEnumerable<string> GetAllErrors()
        {
            var errors = new List<string>();

            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var error = this[property.Name];
                if (!string.IsNullOrEmpty(error))
                {
                    errors.Add(error);
                }
            }

            return errors;
        }
    }
}
