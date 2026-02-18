using EffortGroup.ApplicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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
using System.Windows.Threading;

namespace EffortGroup.Admin
{
    /// <summary>
    /// Логика взаимодействия для PageAddEmp.xaml
    /// </summary>
    public partial class PageAddEmp 
    {
        private bool _passwordVisible = false;
        private bool _confirmPasswordVisible = false;
        public event Action OnEmployeeAdded; // Событие для оповещения об добавлении


        public PageAddEmp()
        {
            InitializeComponent();
            //PasswordTextBox.Visibility = Visibility.Collapsed;
            //ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
        /*private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            _passwordVisible = !_passwordVisible;

            Password.Visibility = _passwordVisible ? Visibility.Collapsed : Visibility.Visible;
            PasswordTextBox.Visibility = _passwordVisible ? Visibility.Visible : Visibility.Collapsed;

            if (_passwordVisible)
            {
                PasswordTextBox.Text = Password.Password;

            }
            else
            {
                PasswordTextBox.Text = "";
            }
            Password_PasswordChanged(sender, e);

        }
        private void ToggleConfirmPasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            _confirmPasswordVisible = !_confirmPasswordVisible;

            ConfirmPassword.Visibility = _confirmPasswordVisible ? Visibility.Collapsed : Visibility.Visible;
            ConfirmPasswordTextBox.Visibility = _confirmPasswordVisible ? Visibility.Visible : Visibility.Collapsed;

            if (_confirmPasswordVisible)
            {
                ConfirmPasswordTextBox.Text = ConfirmPassword.Password;
            }
            else
            {
                ConfirmPasswordTextBox.Text = "";
            }
            Password_PasswordChanged(sender, e);
        }
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string currentPassword = Password.Visibility == Visibility.Visible ? Password.Password : PasswordTextBox.Text;
            string currentConfirmPassword = ConfirmPassword.Visibility == Visibility.Visible ? ConfirmPassword.Password : ConfirmPasswordTextBox.Text;

            if (!string.IsNullOrEmpty(currentPassword) && currentPassword == currentConfirmPassword)
            {
                Add.IsEnabled = true;
                Password.Background = Brushes.LightGreen;
                Password.BorderBrush = Brushes.Green;
                PasswordTextBox.Background = Brushes.LightGreen;
                PasswordTextBox.BorderBrush = Brushes.Green;
                ConfirmPassword.Background = Brushes.LightGreen;
                ConfirmPassword.BorderBrush = Brushes.Green;
                ConfirmPasswordTextBox.Background = Brushes.LightGreen;
                ConfirmPasswordTextBox.BorderBrush = Brushes.Green;
            }
            else
            {
                Add.IsEnabled = false;
                Password.Background = Brushes.LightCoral;
                Password.BorderBrush = Brushes.Red;
                PasswordTextBox.Background = Brushes.LightCoral;
                PasswordTextBox.BorderBrush = Brushes.Red;
                ConfirmPassword.Background = Brushes.LightCoral;
                ConfirmPassword.BorderBrush = Brushes.Red;
                ConfirmPasswordTextBox.Background = Brushes.LightCoral;
                ConfirmPasswordTextBox.BorderBrush = Brushes.Red;
            }
        }*/
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(fio.Text) || string.IsNullOrEmpty(Phone.Text) || string.IsNullOrEmpty(PassNumb.Text) || string.IsNullOrEmpty(PassSeries.Text) || /*string.IsNullOrEmpty(Password.Password) || string.IsNullOrEmpty(ConfirmPassword.Password) string.IsNullOrEmpty(login.Text)*/  string.IsNullOrEmpty(IdDep.Text) || string.IsNullOrEmpty(Post.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    Employee employeeObj = new Employee()
                    {
                        FullName = fio.Text,
                        Phone = Phone.Text,
                        PassNumb = PassNumb.Text,
                        PassSeries = PassSeries.Text,
                        IdDep = int.Parse(IdDep.Text),
                        Post = Post.Text,
                    };
                    context.Employee.Add(employeeObj);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не добавлены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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