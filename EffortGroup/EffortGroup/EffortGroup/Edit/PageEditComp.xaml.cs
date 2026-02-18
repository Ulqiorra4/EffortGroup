using EffortGroup.Admin;
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
    /// Логика взаимодействия для PageEditComp.xaml
    /// </summary>

        public partial class PageEditComp : Page
        {
            private string _imagePath;
            private bool _isAvailable = false;
            private int _computerId;
            private string _selectedPhotoComp;
            private object _currentComputer;

            public PageEditComp()
            {
                InitializeComponent();
            }
            public PageEditComp(int computerId)
            {
                InitializeComponent();
                _computerId = computerId;
                Loaded += PageEditComp_Loaded;
            }
            private async void PageEditComp_Loaded(object sender, RoutedEventArgs e)
            {
                await LoadComputerData();
            }
        private async Task LoadComputerData()
        {
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    var computer = context.Computers.Find(_computerId);
                    if (computer == null)
                    {
                        MessageBox.Show("Компьютер не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Преобразование int в string
                    IdCompTextBox.Text = computer.IdComp.ToString(); // Исправлено
                    BrandTextBox.Text = computer.Brand;
                    ModelTextBox.Text = computer.Model;
                    IdEmpTextBox.Text = computer.IdEmp.ToString(); // Здесь также преобразование int в string
                    _isAvailable = (bool)computer.Availability;
                    _imagePath = computer.PhotoComp; // Загружаем путь из БД

                    // Загружаем изображение, если путь существует
                    if (!string.IsNullOrEmpty(_imagePath))
                    {
                        try
                        {
                            BitmapImage bitmap = new BitmapImage(new Uri(_imagePath, UriKind.Absolute));
                            ComputerImage.Source = bitmap;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    RepairComboBox.SelectedIndex = computer.InRepair ? 0 : 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не загружены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadComputerDataAsync()
            {
                try
                {
                    using (var context = new EffortGroupEntities())
                    {
                        var computer = context.Computers.Find(_computerId);
                        if (computer == null)
                        {
                            MessageBox.Show("Компьютер не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        IdCompTextBox.Text = computer.IdComp.ToString();
                        BrandTextBox.Text = computer.Brand;
                        ModelTextBox.Text = computer.Model;
                        IdEmpTextBox.Text = computer.IdEmp.ToString();
                        _isAvailable = (bool)computer.Availability;
                        _imagePath = computer.PhotoComp; // Загружаем путь из БД

                        // Загружаем изображение, если путь существует
                        if (!string.IsNullOrEmpty(_imagePath))
                        {
                            try
                            {
                                BitmapImage bitmap = new BitmapImage(new Uri(_imagePath, UriKind.Absolute));
                                ComputerImage.Source = bitmap;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    RepairComboBox.SelectedIndex = computer.InRepair ? 0 : 1;
                }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Данные не загружены. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    _selectedPhotoComp = openFileDialog.FileName; // Сохраняем путь в _selectedPhotoComp
                    BitmapImage bitmap = new BitmapImage(new Uri(_selectedPhotoComp, UriKind.Absolute));
                    ComputerImage.Source = bitmap;
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
                            var computer = context.Computers.Find(_computerId);
                            if (computer != null)
                            {
                                computer.Brand = BrandTextBox.Text;
                                computer.Model = ModelTextBox.Text;
                                if (int.TryParse(IdEmpTextBox.Text, out int idEmp))
                                {
                                    computer.IdEmp = idEmp;
                                }
                                else
                                {
                                    MessageBox.Show("Введите корректный идентификатор сотрудника.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                            computer.InRepair = RepairComboBox.SelectedIndex == 0;

                                if (!string.IsNullOrEmpty(_selectedPhotoComp))
                                {
                                    if (!File.Exists(_selectedPhotoComp))
                                    {
                                        MessageBox.Show("Файл по указанному пути не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                    computer.PhotoComp = _selectedPhotoComp;
                                    _imagePath = _selectedPhotoComp; // Обновляем _imagePath после успешного сохранения
                                }
                                context.SaveChanges();
                                MessageBox.Show("Данные компьютера сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                NavigationService?.GoBack();
                            }
                            else
                            {
                                MessageBox.Show("Компьютер не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        int id = _computerId;
                        var computer = context.Computers.Find(id);
                        if (computer == null)
                        {
                            MessageBox.Show("Компьютер не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        context.Computers.Remove(computer);
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

            private void AvailabilityCheckBox_Checked(object sender, RoutedEventArgs e)
            {
                _isAvailable = true; // Когда чекбокс отмечен, _isAvailable = true
            }
            private void AvailabilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
            {
                _isAvailable = false; // Когда чекбокс снят, _isAvailable = false
            }
        }
    }
