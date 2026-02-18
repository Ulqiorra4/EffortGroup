using EffortGroup.ApplicationData;
using Microsoft.Win32;
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

namespace EffortGroup.Admin
{
    /// <summary>
    /// Логика взаимодействия для PageEditPer.xaml
    /// </summary>
    public partial class PageEditPer : Page
    {
        private int peripheralId;
        private int _peripheralId;
        private string _selectedPhotoPer;

        public PageEditPer(int id)
        {
            InitializeComponent();
            peripheralId = id;
            LoadPeripheralData();
        }

        private void LoadPeripheralData()
        {
            using (var context = new EffortGroupEntities())
            {
                var peripheral = context.Peripherials.FirstOrDefault(p => p.IdPer == peripheralId);
                if (peripheral != null)
                {
                    NameTextBox.Text = peripheral.NamePer;
                    TypeTextBox.Text = peripheral.Type;
                    PhotoTextBox.Text = peripheral.PhotoPer;
                }
                else
                {
                    MessageBox.Show("Периферийное устройство не найдено.");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new EffortGroupEntities())
            {
                var peripheral = context.Peripherials.FirstOrDefault(p => p.IdPer == peripheralId);
                if (peripheral != null)
                {
                    peripheral.NamePer = NameTextBox.Text;
                    peripheral.Type = TypeTextBox.Text;
                    peripheral.PhotoPer = PhotoTextBox.Text;

                    context.SaveChanges();
                    MessageBox.Show("Периферийное устройство успешно сохранено.");
                    NavigationService.GoBack(); // Возвращаемся на предыдущую страницу
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении. Периферийное устройство не найдено.");
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    // Предположим, что у вас есть переменная для хранения ID периферийного устройства
                    int id = _peripheralId; // Замените на правильное поле ID вашего устройства

                    var peripheral = context.Peripherials.Find(id); // Замените Computers на Peripherals
                    if (peripheral == null)
                    {
                        MessageBox.Show("Периферийное устройство не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    context.Peripherials.Remove(peripheral); // Замените Computers на Peripherals
                    context.SaveChanges();

                    MessageBox.Show("Данные удалены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frameMain.GoBack(); // Возвращение на предыдущую страницу
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не удалены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPhotoPer = openFileDialog.FileName; // Сохраняем путь в _selectedPhotoComp
                BitmapImage bitmap = new BitmapImage(new Uri(_selectedPhotoPer, UriKind.Absolute));
                PeripheralImage.Source = bitmap;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Возвращаемся на предыдущую страницу
        }
    }
}