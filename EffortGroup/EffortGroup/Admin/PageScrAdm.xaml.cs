using EffortGroup.Admin;
using EffortGroup.ApplicationData;
using System;
using System.Collections.Generic;
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

namespace EffortGroup.MenuScrin
{
    /// <summary>
    /// Логика взаимодействия для PageScrAdm.xaml
    /// </summary>
    public partial class PageScrAdm : Page
    {
        public PageScrAdm()
        {
            InitializeComponent();
        }

        private async void Employee_Click(object sender, RoutedEventArgs e)
        {
            // Создаем страницу PageAdmin
            var pageAdmin = new PageAdmin
            {
                // Вызываем метод загрузки данных, устанавливаем showingEmployees = true
                showingEmployees = true // Устанавливаем значение showingEmployees
            };
            await pageAdmin.LoadEmployeeDataAsync(); // Вызов асинхронного метода загрузки

            // Переходим на страницу PageAdmin
            NavigationService.Navigate(pageAdmin);

        }


        private async void CompTech_Click(object sender, RoutedEventArgs e)
        {
            // Создаем страницу PageAdmin
            var pageAdmin = new PageAdmin();

            // Устанавливаем значение showingEmployees = false
            pageAdmin.showingEmployees = false;

            // Вызываем метод загрузки данных, так же асинхронно
            await pageAdmin.LoadComputerDataAsync();

            // Переходим на страницу PageAdmin
            NavigationService.Navigate(pageAdmin);
        }

        private async void Per_Click(object sender, RoutedEventArgs e)
        {
            // Создаем страницу PageAdmin
            var pageAdmin = new PageAdmin();

            // Устанавливаем значение showingEmployees = false
            pageAdmin.showingEmployees = false;

            // Вызываем метод загрузки данных так же асинхронно
            await pageAdmin.LoadPeripheralDataAsync();

            // Переходим на страницу PageAdmin
            NavigationService.Navigate(pageAdmin);
        }
    }
}