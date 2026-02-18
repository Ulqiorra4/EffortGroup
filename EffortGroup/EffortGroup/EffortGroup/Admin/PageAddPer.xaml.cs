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
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
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
            if (string.IsNullOrEmpty(Type.Text) || string.IsNullOrEmpty(NamePer.Text) || string.IsNullOrEmpty(IdComp.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    Peripherials peripheralObj = new Peripherials()
                    {
                        Type = Type.Text,
                        NamePer = NamePer.Text,
                        IdComp = int.Parse(IdComp.Text)
                    };
                    if (!string.IsNullOrEmpty(_imagePath))
                    {
                        if (!File.Exists(_imagePath))
                        {
                            MessageBox.Show("Файл по указанному пути не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        peripheralObj.PhotoPer = _imagePath;
                    }
                    context.Peripherials.Add(peripheralObj);
                    context.SaveChanges();
                    OnPeripheralAdded?.Invoke(); // Вызываем событие
                    MessageBox.Show("Данные сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frameMain.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не добавлены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}