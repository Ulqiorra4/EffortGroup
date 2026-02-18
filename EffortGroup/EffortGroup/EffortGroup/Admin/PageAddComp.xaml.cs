using EffortGroup.ApplicationData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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

namespace EffortGroup.Admin
{
    /// <summary>
    /// Логика взаимодействия для PageAddComp.xaml
    /// </summary>
    public partial class PageAddComp : Page
    {
        private int _id;

        public event Action OnComputerAdded;

        public PageAddComp(int id)
        {
            InitializeComponent();
        }

        public PageAddComp()
        {
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
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();
                PhotoComp.Source = bitmap;
            }
        }
        public void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Brand.Text) || string.IsNullOrEmpty(Model.Text) || string.IsNullOrEmpty(IdEmp.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    Computers computerObj = new Computers()
                    {
                        Brand = Brand.Text,
                        Model = Model.Text,
                        IdEmp = int.Parse(IdEmp.Text)
                    };
                    context.Computers.Add(computerObj);
                    context.SaveChanges();
                    OnComputerAdded?.Invoke(); // Вызываем событие
                    MessageBox.Show("Данные сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frameMain.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не добавлены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}