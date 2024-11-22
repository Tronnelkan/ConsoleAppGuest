// WpfApp/ViewModels/BaseViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WpfApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // Реализация IDataErrorInfo для валидации
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                var validationContext = new ValidationContext(this)
                {
                    MemberName = columnName
                };
                var results = new List<ValidationResult>();
                var property = GetType().GetProperty(columnName);
                if (property == null)
                    return null;
                var value = property.GetValue(this);
                bool isValid = Validator.TryValidateProperty(value, validationContext, results);
                if (isValid)
                    return null;
                return results.First().ErrorMessage;
            }
        }

        public bool HasErrors
        {
            get
            {
                var properties = GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (!string.IsNullOrEmpty(this[property.Name]))
                        return true;
                }
                return false;
            }
        }

        // Метод для валидации всех свойств
        public void ValidateAllProperties()
        {
            foreach (var property in GetType().GetProperties())
            {
                var error = this[property.Name];
                if (!string.IsNullOrEmpty(error))
                {
                    // Здесь можно логировать или обрабатывать ошибки
                }
            }
        }
    }
}
