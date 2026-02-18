using EffortGroup.Admin;
using EffortGroup.ApplicationData;
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

namespace EffortGroup.Employee
{
    /// <summary>
    /// Логика взаимодействия для PageScrEmp.xaml
    /// </summary>
    public partial class PageScrEmp : Page
    {
        public PageScrEmp()
        {
            InitializeComponent();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
        private async void CompTech_Click(object sender, RoutedEventArgs e)
        {
            // Создаем страницу PageEmployee
            var pageEmployee = new PageEmployee();

            // Устанавливаем значение showingPeriph = false
            pageEmployee.showingPeriph = false;

            // Вызываем метод загрузки данных, так же асинхронно
            await pageEmployee.LoadComputerDataAsync();

            // Переходим на страницу PageEmployee
            NavigationService.Navigate(pageEmployee);
        }

        private async void Per_Click(object sender, RoutedEventArgs e)
        {
            // Создаем страницу PageEmployee
            var pageEmployee = new PageEmployee();

            // Устанавливаем значение showingEmployees = false
            pageEmployee.showingPeriph = false;

            // Вызываем метод загрузки данных так же асинхронно
            await pageEmployee.LoadPeripheralDataAsync();

            // Переходим на страницу PageEmployee
            NavigationService.Navigate(pageEmployee);
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
