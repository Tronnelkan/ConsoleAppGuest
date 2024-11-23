using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Domain.Models;
using GuestWPF.Commands;

namespace GuestWPF.ViewModels
{
    public class AddAddressViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private string _street;
        private string _city;
        private string _country;

        public string Street
        {
            get => _street;
            set { _street = value; OnPropertyChanged(); ValidateProperty(value, nameof(Street)); }
        }

        public string City
        {
            get => _city;
            set { _city = value; OnPropertyChanged(); ValidateProperty(value, nameof(City)); }
        }

        public string Country
        {
            get => _country;
            set { _country = value; OnPropertyChanged(); ValidateProperty(value, nameof(Country)); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<Address>? RequestClose;

        public AddAddressViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void Save()
        {
            var address = new Address
            {
                Street = this.Street,
                City = this.City,
                Country = this.Country
            };

            RequestClose?.Invoke(this, address);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(this, null);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Street) &&
                   !string.IsNullOrWhiteSpace(City) &&
                   !string.IsNullOrWhiteSpace(Country) &&
                   !HasErrors;
        }

        #region Validation

        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return Enumerable.Empty<string>();

            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];

            return Enumerable.Empty<string>();
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ValidateProperty(object value, string propertyName)
        {
            var results = new List<string>();

            switch (propertyName)
            {
                case nameof(Street):
                    if (string.IsNullOrWhiteSpace(Street))
                    {
                        results.Add("Вулиця не може бути порожньою.");
                    }
                    break;

                case nameof(City):
                    if (string.IsNullOrWhiteSpace(City))
                    {
                        results.Add("Місто не може бути порожнім.");
                    }
                    break;

                case nameof(Country):
                    if (string.IsNullOrWhiteSpace(Country))
                    {
                        results.Add("Країна не може бути порожньою.");
                    }
                    break;
            }

            if (results.Any())
            {
                _errors[propertyName] = results;
            }
            else
            {
                _errors.Remove(propertyName);
            }

            OnErrorsChanged(propertyName);
        }

        #endregion
    }
}
