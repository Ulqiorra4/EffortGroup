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
        private int _peripheralId;
        private Peripherals _currentPeripheral;
        private string _selectedPhotoPer;
        private object[] _periphId;
        private string _imagePath;
        private bool _isAvailable;

        public PageEditPer(int peripheralId)
        {
            InitializeComponent();
            _peripheralId = peripheralId;
            DataContext = this;
            if (RepairComboBox.Items.Count == 0)
            {
                RepairComboBox.Items.Add("Да");
                RepairComboBox.Items.Add("Нет");
            }
            LoadPeripheralDataAsync();
        }


        private async void PageEditPer_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPeripheralDataAsync();
        }
        private async Task LoadPeripheralDataAsync()
        {
            using (var context = new EffortGroupEntities())
            {
                var peripheral = context.Peripherals.FirstOrDefault(per => per.IdPer == _peripheralId);
                if (peripheral != null)
                {
                    _currentPeripheral = new Peripherals
                    {
                        IdPer = peripheral.IdPer,
                        Type = peripheral.Type, // Загружаем Type
                        NamePer = peripheral.NamePer,
                        IdComp = peripheral.IdComp,
                        PhotoPer = peripheral.PhotoPer
                    };

                    NamePer.Text = peripheral.NamePer; // Имя в NamePerTextBox
                    Type.Text = peripheral.Type; // Тип в TypePerTextBox
                    IdCompTextBox.Text = peripheral.IdComp.ToString(); // ID компьютера в IdCompTextBox

                }
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
                PerImage.Source = bitmap;
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
                        var periph = context.Peripherals.Find(_peripheralId);
                        if (periph != null)
                        {
                            periph.Type = Type.Text;
                            periph.NamePer = NamePer.Text;

                            if (int.TryParse(IdCompTextBox.Text, out int idComp))
                            {
                                periph.IdComp = idComp;
                            }
                            else
                            {
                                MessageBox.Show("Введите корректный идентификатор компьютера.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            if (!string.IsNullOrEmpty(_selectedPhotoPer))
                            {
                                if (!File.Exists(_selectedPhotoPer))
                                {
                                    MessageBox.Show("Файл по указанному пути не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                                periph.PhotoPer = _selectedPhotoPer;
                                _imagePath = _selectedPhotoPer; // Обновляем _imagePath после успешного сохранения
                            }
                            string selectedRepairStatus = RepairComboBox.SelectedItem as string;
                            var existingRepair = context.DeviceRepare.FirstOrDefault(r => r.IdPer == _peripheralId);
                            DateTime currentDate = DateTime.Now;
                            if (existingRepair == null)
                            {
                                if (!string.IsNullOrEmpty(selectedRepairStatus))
                                {
                                    var newRepair = new DeviceRepare
                                    {
                                        IdPer = _peripheralId,
                                        Status = selectedRepairStatus,
                                        Description = selectedRepairStatus == "Да" ? "Ремонт начат" : "Ремонт окончен",
                                        StartDataRep = selectedRepairStatus == "Да" ? (DateTime?)currentDate : null,
                                        EndDataRep = selectedRepairStatus == "Нет" ? (DateTime?)currentDate : null
                                    };
                                    context.DeviceRepare.Add(newRepair);
                                    MessageBox.Show(selectedRepairStatus == "Да" ? $"Ремонт начат. Дата: {currentDate}" : $"Ремонт окончен. Дата: {currentDate}", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(selectedRepairStatus))
                                {
                                    existingRepair.Status = selectedRepairStatus;
                                    existingRepair.Description = selectedRepairStatus == "Да" ? "Ремонт начат" : "Ремонт окончен";
                                    if (selectedRepairStatus == "Да")
                                    {
                                        existingRepair.StartDataRep = currentDate;
                                        existingRepair.EndDataRep = null;
                                    }
                                    else
                                    {
                                        existingRepair.EndDataRep = currentDate;
                                        existingRepair.StartDataRep = null;
                                    }
                                    MessageBox.Show(selectedRepairStatus == "Да" ? $"Ремонт начат. Дата: {currentDate}" : $"Ремонт окончен. Дата: {currentDate}", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                            context.SaveChanges();
                            NavigationService?.GoBack();
                        }
                        else
                        {
                            MessageBox.Show("Периферия не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    int id = _peripheralId;
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