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

namespace EffortGroup.Admin
{
    /// <summary>
    /// Логика взаимодействия для PageEditPer.xaml
    /// </summary>
    public partial class PageEditPer : Page
    {
        private string _imagePath;
        private int _peripheralId;
        private string _selectedPhotoPer;
        private object _currentPeripheral;

        public PageEditPer()
        {
            InitializeComponent();
        }

        public PageEditPer(int peripheralId)
        {
            InitializeComponent();
            _peripheralId = peripheralId;
            Loaded += PageEditPer_Loaded;
        }

        private async void PageEditPer_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPeripheralData();
        }

        private async Task LoadPeripheralData()
        {
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    var peripheral = context.Peripherials.Find(_peripheralId);
                    if (peripheral == null)
                    {
                        MessageBox.Show("Периферийное устройство не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Преобразование int в string
                    IdPer.Text = peripheral.IdPer.ToString(); // Исправлено
                    Type.Text = peripheral.Type;
                    NamePer.Text = peripheral.NamePer;
                    IdComp.Text = peripheral.IdComp.ToString(); // Здесь также преобразование int в string
                    _imagePath = peripheral.PhotoPer; // Загружаем путь из БД

                    // Загружаем изображение, если путь существует
                    if (!string.IsNullOrEmpty(_imagePath))
                    {
                        try
                        {
                            BitmapImage bitmap = new BitmapImage(new Uri(_imagePath, UriKind.Absolute));
                            PhotoPer.Source = bitmap;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не загружены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPhotoPer = openFileDialog.FileName; // Сохраняем путь в _selectedPhotoPer
                BitmapImage bitmap = new BitmapImage(new Uri(_selectedPhotoPer, UriKind.Absolute));
                PhotoPer.Source = bitmap;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new EffortGroupEntities())
                    {
                        var peripheral = context.Peripherials.Find(_peripheralId);
                        if (peripheral != null)
                        {
                            peripheral.Type = Type.Text;
                            peripheral.NamePer = NamePer.Text;

                            // Проверяем, введено ли значение для IdComp
                            if (string.IsNullOrWhiteSpace(IdComp.Text))
                            {
                                peripheral.IdComp = null; // Устанавливаем значение в null, если поле пустое
                            }
                            else if (int.TryParse(IdComp.Text, out int idComp))
                            {
                                peripheral.IdComp = idComp; // Устанавливаем значение, если оно корректное
                            }
                            else
                            {
                                MessageBox.Show("Введите корректный идентификатор компьютера или оставьте поле пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // Проверка на наличие выбранного изображения
                            if (!string.IsNullOrEmpty(_selectedPhotoPer))
                            {
                                if (!File.Exists(_selectedPhotoPer))
                                {
                                    MessageBox.Show("Файл по указанному пути не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                                peripheral.PhotoPer = _selectedPhotoPer;
                                _imagePath = _selectedPhotoPer; // Обновляем _imagePath после успешного сохранения
                            }
                            context.SaveChanges();
                            MessageBox.Show("Данные периферийного устройства сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            PageAdmin pageAdmin = new PageAdmin();
                            pageAdmin.CreateButtons(); // Вызов метода для обновления интерфейса
                        }
                        else
                        {
                            MessageBox.Show("Периферийное устройство не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Данные не обновлены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    var peripheral = context.Peripherials.Find(_peripheralId);
                    if (peripheral == null)
                    {
                        MessageBox.Show("Периферийное устройство не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    context.Peripherials.Remove(peripheral);
                    context.SaveChanges();
                    MessageBox.Show("Данные удалены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frameMain.GoBack();
                    PageAdmin pageAdmin = new PageAdmin();
                    pageAdmin.CreateButtons(); // Вызов метода
                    NavigationService?.GoBack();
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
    }
}
