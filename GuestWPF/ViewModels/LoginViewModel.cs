using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CourseProject.BLL.Services;
using GuestWPF.Commands;
using GuestWPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GuestWPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;

        private string _login;
        private string _password;

        /// <summary>
        /// Властивість для зберігання логіну користувача.
        /// </summary>
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Властивість для зберігання пароля користувача.
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Команда для виконання входу.
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Команда для відкриття вікна реєстрації.
        /// </summary>
        public ICommand RegisterCommand { get; }

        /// <summary>
        /// Команда для відкриття вікна відновлення пароля.
        /// </summary>
        public ICommand ForgotPasswordCommand { get; }

        /// <summary>
        /// Конструктор LoginViewModel.
        /// </summary>
        /// <param name="userService">Сервіс для роботи з користувачами.</param>
        /// <param name="serviceProvider">Сервіс-провайдер для DI.</param>
        public LoginViewModel(IUserService userService, IServiceProvider serviceProvider)
        {
            _userService = userService;
            _serviceProvider = serviceProvider;

            // Ініціалізація команд
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin());
            RegisterCommand = new RelayCommand(_ => OpenRegisterWindow(), _ => true);
            ForgotPasswordCommand = new RelayCommand(_ => OpenForgotPasswordWindow(), _ => true);
        }

        /// <summary>
        /// Перевіряє, чи можна виконати команду Login.
        /// </summary>
        /// <returns>True, якщо обидва поля заповнені; інакше False.</returns>
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
        }

        /// <summary>
        /// Асинхронний метод для виконання логіки входу.
        /// </summary>
        private async Task LoginAsync()
        {
            try
            {
                // Повідомлення для діагностики
                MessageBox.Show("LoginCommand Executed", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);

                bool isAuthenticated = await _userService.AuthenticateAsync(Login, Password);
                if (isAuthenticated)
                {
                    MessageBox.Show("Ви успішно ввійшли!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Відкриття головного вікна через DI
                    var mainWindow = _serviceProvider.GetService<MainWindow>();
                    if (mainWindow != null)
                    {
                        mainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Не вдалося відкрити головне вікно.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Закриття вікна входу
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Невірний логін або пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка під час входу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Метод для відкриття вікна реєстрації.
        /// </summary>
        private void OpenRegisterWindow()
        {
            try
            {
                // Повідомлення для діагностики
                MessageBox.Show("RegisterCommand Executed", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);

                var registerWindow = _serviceProvider.GetService<RegisterWindow>();
                if (registerWindow != null)
                {
                    registerWindow.Show();

                    // Опціонально: Закриття вікна входу після відкриття вікна реєстрації
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Не вдалося відкрити вікно реєстрації.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка при відкритті вікна реєстрації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Метод для відкриття вікна відновлення пароля.
        /// </summary>
        private void OpenForgotPasswordWindow()
        {
            try
            {
                // Повідомлення для діагностики
                MessageBox.Show("ForgotPasswordCommand Executed", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);

                var forgotPasswordWindow = _serviceProvider.GetService<ForgotPasswordWindow>();
                if (forgotPasswordWindow != null)
                {
                    forgotPasswordWindow.Show();

                    // Опціонально: Закриття вікна входу після відкриття вікна відновлення пароля
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Не вдалося відкрити вікно відновлення пароля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка при відкритті вікна відновлення пароля: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
