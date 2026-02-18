using EffortGroup.ApplicationData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace EffortGroup
{
    /// <summary>
    /// Логика взаимодействия для PageAddPer.xaml
    /// </summary>
    public partial class PageAddPer : Page
    {
        private string _imagePath;
        private int _peripheralId;
        public event Action OnPeripheralAdded;

        public PageAddPer()
        {
            InitializeComponent();
        }

        public PageAddPer(int peripheralId)
        {
            InitializeComponent();
            _peripheralId = peripheralId;
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
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_imagePath);
                bitmap.EndInit();
                PhotoPer.Source = bitmap;
            }
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем на обязательные поля
            if (string.IsNullOrWhiteSpace(Type.Text) || string.IsNullOrWhiteSpace(NamePer.Text))
            {
                MessageBox.Show("Заполните все обязательные поля (Тип и Название).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new EffortGroupEntities())
                {
                    var peripheralObj = new Peripherals
                    {
                        Type = Type.Text,
                        NamePer = NamePer.Text,
                    };

                    // Обработка IdComp
                    if (string.IsNullOrWhiteSpace(IdComp.Text))
                    {
                        peripheralObj.IdComp = null; // Или peripheralObj.IdComp = 0;
                    }
                    else if (int.TryParse(IdComp.Text, out int idComp))
                    {
                        peripheralObj.IdComp = idComp;
                    }
                    else
                    {
                        MessageBox.Show("Введите корректный идентификатор компьютера.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Обработка PhotoPer
                    if (!string.IsNullOrEmpty(_imagePath))
                    {
                        if (!File.Exists(_imagePath))
                        {
                            MessageBox.Show("Файл по указанному пути не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        peripheralObj.PhotoPer = _imagePath;
                    }


                    context.Peripherals.Add(peripheralObj);
                    context.SaveChanges();

                    OnPeripheralAdded?.Invoke(); // Вызываем событие после сохранения

                    MessageBox.Show("Данные сохранены.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frameMain.GoBack(); // Возвращаемся на предыдущую страницу
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не добавлены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}