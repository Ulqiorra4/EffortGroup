using EffortGroup.ApplicationData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
        private string _selectedPhotoComp;
        private ApplicationData.Computers _currentComp;
        private string _imagePath;

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
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPhotoComp = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedPhotoComp);
                bitmap.EndInit();
                PhotoComp.Source = bitmap;
            }
        }
        public void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Brand.Text) || string.IsNullOrEmpty(Model.Text))
            {
                MessageBox.Show("Заполните все поля, кроме ID Сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    // Проверяем, существует ли компьютер с указанным IdComp

                    Computers comp = string.IsNullOrEmpty(IdEmp.Text) ? null : context.Computers.Find(int.Parse(IdEmp.Text));
                    if (string.IsNullOrEmpty(IdEmp.Text) || comp != null)
                    {
                        Computers computerObj = new Computers()
                        {
                            Brand = Brand.Text,
                            Model = Model.Text,
                            IdEmp = string.IsNullOrEmpty(IdEmp.Text) ? (int?)null : int.Parse(IdEmp.Text)
                        };

                        // Добавляем новый компьютер в контекст
                        context.Computers.Add(computerObj);
                        context.SaveChanges();

                        OnComputerAdded?.Invoke(); // Вызываем событие
                        MessageBox.Show("Данные сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                        AppFrame.frameMain.GoBack();
                        PageAdmin pageAdmin = new PageAdmin();
                        pageAdmin.CreateButtons(); // Вызов метода
                    }
                    else
                    {
                        MessageBox.Show("Компьютер с указанным IdComp не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show("Данные не добавлены. Произошла ошибка при обновлении записей. Дополнительные сведения: " + dbEx.InnerException?.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show("Пожалуйста, введите корректное значение для Id сотрудника. Ошибка: " + formatEx.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            /*try
            {
                using (var context = new EffortGroupEntities())
                {
                    var comp = context.Computers.FirstOrDefault(c => c.IdComp == _currentComp.IdComp);
                    Computers computerObj = new Computers()
                    {
                        Brand = Brand.Text,
                        Model = Model.Text,
                        IdEmp = string.IsNullOrEmpty(IdEmp.Text) ? (int?)null : int.Parse(IdEmp.Text)

                    };
                    context.Computers.Add(computerObj);
                    context.SaveChanges();
                    OnComputerAdded?.Invoke(); // Вызываем событие
                    MessageBox.Show("Данные сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frameMain.GoBack();
                    PageAdmin pageAdmin = new PageAdmin();
                    pageAdmin.CreateButtons(); // Вызов метода
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не добавлены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
