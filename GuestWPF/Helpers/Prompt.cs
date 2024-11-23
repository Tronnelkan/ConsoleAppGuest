using System.Windows;
using System.Windows.Controls;

namespace GuestWPF.Helpers
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            var prompt = new Window()
            {
                Width = 400,
                Height = 150,
                Title = caption,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new System.Windows.Controls.StackPanel() { Margin = new Thickness(10) };

            var textBlock = new System.Windows.Controls.TextBlock() { Text = text };
            var textBox = new System.Windows.Controls.TextBox() { Margin = new Thickness(0, 5, 0, 5) };

            var buttonPanel = new System.Windows.Controls.StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var okButton = new System.Windows.Controls.Button() { Content = "OK", Width = 75, Margin = new Thickness(5, 0, 0, 0) };
            var cancelButton = new System.Windows.Controls.Button() { Content = "Відміна", Width = 75 };

            okButton.Click += (sender, e) => { prompt.DialogResult = true; prompt.Close(); };
            cancelButton.Click += (sender, e) => { prompt.DialogResult = false; prompt.Close(); };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(buttonPanel);

            prompt.Content = stackPanel;
            bool? result = prompt.ShowDialog();

            return result == true ? textBox.Text : null;
        }
    }
}
