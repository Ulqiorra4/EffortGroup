using EffortGroup.ApplicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
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
            // Проверяем, заполнены ли все обязательные поля
            if (string.IsNullOrEmpty(fio.Text) ||
                string.IsNullOrEmpty(Phone.Text) ||
                string.IsNullOrEmpty(PassNumb.Text) ||
                string.IsNullOrEmpty(PassSeries.Text) ||
                string.IsNullOrEmpty(IdDep.Text) ||
                string.IsNullOrEmpty(Post.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new EffortGroupEntities())
                {
                    // Сокращаем FullName до формата "Фамилия И.О."
                    string[] nameParts = fio.Text.Split(' ');
                    string lastName = nameParts[0]; // Фамилия
                    string firstInitial = nameParts.Length > 1 ? nameParts[1].Substring(0, 1) : ""; // Первая буква имени
                    string middleInitial = nameParts.Length > 2 ? nameParts[2].Substring(0, 1) : ""; // Первая буква отчества

                    string abbreviatedName = $"{lastName} {firstInitial}.{middleInitial}.";

                    // Создаем новый объект Employee
                    ApplicationData.Employee employeeObj = new ApplicationData.Employee()
                    {
                        FullName = abbreviatedName,
                        Phone = Phone.Text,
                        PassNumb = PassNumb.Text,
                        PassSeries = PassSeries.Text,
                        IdDep = int.Parse(IdDep.Text),
                        Post = Post.Text,
                    };

                    // Добавляем нового сотрудника в контекст
                    context.Employee.Add(employeeObj);
                    context.SaveChanges(); // Сохраняем изменения в базе данных

                    // Вызываем событие, если оно существует (добавьте его в классе, если нужно)
                    OnEmployeeAdded?.Invoke(); // Если у вас есть событие OnEmployeeAdded
                    MessageBox.Show("Данные сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    PageAdmin pageAdmin = new PageAdmin();
                    pageAdmin.CreateButtons(); // Вызов метода для обновления интерфейса
                }
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show("Данные не добавлены. Произошла ошибка при обновлении записей. Дополнительные сведения: " + dbEx.InnerException?.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show("Пожалуйста, введите корректное значение для IdDep. Ошибка: " + formatEx.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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