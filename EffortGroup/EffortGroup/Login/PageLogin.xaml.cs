using EffortGroup.Admin;
using EffortGroup.ApplicationData;
using EffortGroup.MenuScrin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EffortGroup.Login
{
    public partial class PageLogin : Page
    {
        public PageLogin()
        {
            InitializeComponent();
        }

        private async void Enter_Click(object sender, RoutedEventArgs e)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                User authenticatedUser = await AuthenticateUserAsync(Login.Text, Password.Password);

                if (authenticatedUser == null)
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    HandleSuccessfulLogin(authenticatedUser);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<User> AuthenticateUserAsync(string login, string password)
        {
            // Simulate an asynchronous database call (replace with your actual authentication logic)

            //Simulate a delay for better UX.  Remove in a production application.
            await Task.Delay(500);

            return AppConnect.modelObj.User.FirstOrDefault(x => x.Login == login && x.Password == password);
        }

        private void HandleSuccessfulLogin(User user)
        {
            switch (user.IdRole)
            {
                case 1:
                    AppFrame.frameMain.Navigate(new PageScrAdm());
                    MessageBox.Show("Здравствуйте, Администратор!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    //Navigate to the appropriate page for employees.
                    AppFrame.frameMain.Navigate(new PageEmployee()); //Create this page!
                    MessageBox.Show("Здравствуйте, Сотрудник!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    MessageBox.Show("Не удалось определить роль пользователя.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }

        private void Login_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void Login_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Login.Text))
            {
                LoginPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Password.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected virtual void LogoHyperlink_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://effort-group.ru/") { UseShellExecute = true });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть ссылку: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}