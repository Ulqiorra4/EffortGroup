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
        private Computers _currentComputer;

        public PageEditComp()
        {
            InitializeComponent();
        }
        public PageEditComp(int computerId)
        {
            InitializeComponent();
            _computerId = computerId;
            DataContext = this;
            if (RepairComboBox.Items.Count == 0)
            {
                RepairComboBox.Items.Add("Да");
                RepairComboBox.Items.Add("Нет");
            }
            LoadComputerDataAsync();
        }
        private async void PageEditComp_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadComputerDataAsync();
        }
        private async Task LoadComputerDataAsync()
        {
            using (var context = new EffortGroupEntities())
            {
                var computer = context.Computers.FirstOrDefault(comp => comp.IdComp == _computerId);
                if (computer != null)
                {
                    BrandTextBox.Text = computer.Brand;
                    ModelTextBox.Text = computer.Model;
                    IdEmpTextBox.Text = computer.IdEmp.ToString();
                    var repair = context.DeviceRepare.FirstOrDefault(r => r.IdComp == _computerId);
                    if (repair != null)
                    {
                        if (repair.StartDataRep != null && repair.EndDataRep == null)
                        {
                            RepairComboBox.SelectedItem = "Да";
                        }
                        else if (repair.StartDataRep == null && repair.EndDataRep != null)
                        {
                            RepairComboBox.SelectedItem = "Нет";
                        }
                    }
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
                            string selectedRepairStatus = RepairComboBox.SelectedItem as string;
                            var existingRepair = context.DeviceRepare.FirstOrDefault(r => r.IdComp == _computerId);
                            DateTime currentDate = DateTime.Now;
                            if (existingRepair == null)
                            {
                                if (!string.IsNullOrEmpty(selectedRepairStatus))
                                {
                                    var newRepair = new DeviceRepare
                                    {
                                        IdComp = _computerId,
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
                            // Сохраняем путь к фотографии, если она изменена
                            if (!string.IsNullOrEmpty(_selectedPhotoComp))
                            {
                                if (!File.Exists(_selectedPhotoComp))
                                {
                                    MessageBox.Show("Файл по указанному пути не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                                computer.PhotoComp = _selectedPhotoComp;
                                _imagePath = _selectedPhotoComp;
                            }
                            context.SaveChanges();
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