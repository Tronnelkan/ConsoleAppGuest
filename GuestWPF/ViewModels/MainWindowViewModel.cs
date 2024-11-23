using GuestWPF.Commands;
using GuestWPF.ViewModels;
using GuestWPF.Views;
using GuestWPF;
using System.Windows.Input;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

public class MainWindowViewModel : BaseViewModel
{
    public ICommand ChangePasswordCommand { get; }
    public ICommand ManageUsersCommand { get; }

    public MainWindowViewModel()
    {
        ChangePasswordCommand = new RelayCommand(OpenChangePasswordWindow, _ => true);
        ManageUsersCommand = new RelayCommand(OpenManageUsersWindow, _ => true);
    }

    private void OpenChangePasswordWindow(object parameter)
    {
        // Додайте повідомлення для перевірки
        MessageBox.Show("ChangePasswordCommand Executed", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);

        var forgotPasswordWindow = App.ServiceProvider.GetRequiredService<ForgotPasswordWindow>();
        forgotPasswordWindow.Show();
    }

    private void OpenManageUsersWindow(object parameter)
    {
        // Додайте повідомлення для перевірки
        MessageBox.Show("ManageUsersCommand Executed", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);

        var manageUsersWindow = App.ServiceProvider.GetRequiredService<ManageUsersWindow>();
        manageUsersWindow.Show();
    }
}
