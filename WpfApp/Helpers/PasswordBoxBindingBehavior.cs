using System.Windows;
using System.Windows.Controls;

namespace WpfApp.Helpers
{
    public static class PasswordBoxBindingBehavior
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxBindingBehavior),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            var box = dp as PasswordBox;
            if (box != null)
            {
                box.PasswordChanged -= HandlePasswordChanged;
                box.Password = value;
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        public static string GetBoundPassword(DependencyObject dp)
        {
            var box = dp as PasswordBox;
            return box?.Password;
        }

        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var box = dp as PasswordBox;
            if (box != null)
            {
                box.Password = e.NewValue as string;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as PasswordBox;
            SetBoundPassword(box, box.Password);
        }
    }
}
