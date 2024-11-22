// WpfApp/Helpers/PasswordBoxHelper.cs
using System.Windows;
using System.Windows.Controls;

namespace WpfApp.Helpers
{
    public static class PasswordBoxHelper
    {
        // Присоединяемое свойство для связывания пароля
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        // Присоединяемое свойство для включения/отключения связывания
        public static readonly DependencyProperty BindPassword =
            DependencyProperty.RegisterAttached(
                "BindPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        // Внутреннее присоединяемое свойство для предотвращения рекурсии
        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached(
                "UpdatingPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        // Геттеры и сеттеры для BoundPassword
        public static string GetBoundPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BoundPassword, value);
        }

        // Геттеры и сеттеры для BindPassword
        public static bool GetBindPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(BindPassword);
        }

        public static void SetBindPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(BindPassword, value);
        }

        // Геттеры и сеттеры для UpdatingPassword
        private static bool GetUpdatingPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdatingPassword, value);
        }

        // Обработчик изменения BoundPassword
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // Отписываемся от события, чтобы избежать рекурсии
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

                if (!GetUpdatingPassword(passwordBox))
                {
                    passwordBox.Password = e.NewValue as string ?? string.Empty;
                }

                // Подписываемся на событие снова
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        // Обработчик изменения BindPassword
        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                bool wasBound = (bool)(e.OldValue);
                bool needToBind = (bool)(e.NewValue);

                if (wasBound)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }

                if (needToBind)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
            }
        }

        // Обработчик события PasswordChanged
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetUpdatingPassword(passwordBox, true);
                SetBoundPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }
    }
}
