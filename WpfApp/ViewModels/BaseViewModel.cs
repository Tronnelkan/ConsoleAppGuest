// WpfApp/ViewModels/BaseViewModel.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace WpfApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(err => err.Value);

            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];

            return null;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var validationContext = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(value, validationContext, results);

            if (_errors.ContainsKey(propertyName))
                _errors.Remove(propertyName);

            if (results.Any())
                _errors.Add(propertyName, results.Select(c => c.ErrorMessage).ToList());

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ValidateAllProperties()
        {
            var properties = GetType().GetProperties()
                                     .Where(prop => Attribute.IsDefined(prop, typeof(ValidationAttribute)));

            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                ValidateProperty(value, property.Name);
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
