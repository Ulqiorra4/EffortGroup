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
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace EffortGroup.Admin
{
    /// <summary>
    /// Логика взаимодействия для EditEmployeeWindow.xaml
    /// </summary>
    public partial class EditEmployeeWindow : Window
    {
        private int _employeeId;
        private Employee _currentEmployee;

        public EditEmployeeWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadEmployeeData();
        }
        private void LoadEmployeeData()
        {
            using (var context = new EffortGroupEntities())
            {
                var employee = context.Employee.FirstOrDefault(emp => emp.IdEmp == _employeeId);
                if (employee != null)
                {
                    _currentEmployee = new Employee
                    {
                        IdEmp = employee.IdEmp,
                        FullName = employee.FullName,
                        Phone = employee.Phone,
                        PassNumb = employee.PassNumb,
                        PassSeries = employee.PassSeries,
                        IdDep = employee.IdDep,
                        Post = employee.Post
                    };

                    IdEmpTextBox.Text = _currentEmployee.IdEmp.ToString();
                    FullNameTextBox.Text = _currentEmployee.FullName;
                    PhoneTextBox.Text = _currentEmployee.Phone.ToString();
                    PassNumbTextBox.Text = _currentEmployee.PassNumb.ToString();
                    PassSeriesTextBox.Text = _currentEmployee.PassSeries.ToString();
                    IdDepTextBox.Text = _currentEmployee.IdDep.ToString();
                    PostTextBox.Text = _currentEmployee.Post;
                }
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new EffortGroupEntities())
                {
                    var dbEmployee = context.Employee.FirstOrDefault(emp => emp.IdEmp == _currentEmployee.IdEmp);
                    if (dbEmployee != null)
                    {
                        dbEmployee.FullName = FullNameTextBox.Text;
                        dbEmployee.Phone = PhoneTextBox.Text;
                        dbEmployee.PassNumb = PassNumbTextBox.Text;
                        dbEmployee.PassSeries = PassSeriesTextBox.Text;
                        dbEmployee.IdDep = int.Parse(IdDepTextBox.Text);
                        dbEmployee.Post = PostTextBox.Text;
                        if (!string.IsNullOrEmpty(_selectedPhotoPath))
                        {
                            dbEmployee.PhotoPath = _selectedPhotoPath;
                        }
                        context.SaveChanges();
                        MessageBox.Show($"Данные сотрудника {_currentEmployee.FullName} сохранены.");
                        this.Close();
                    }
                }
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new EffortGroupEntities())
                {
                    var dbEmployee = context.Employee.FirstOrDefault(emp => emp.IdEmp == _currentEmployee.IdEmp);
                    if (dbEmployee != null)
                    {
                        context.Employee.Remove(dbEmployee);
                        context.SaveChanges();
                        MessageBox.Show($"Сотрудник {_currentEmployee.FullName} удален.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Сотрудник для удаления не найден.");
                    }
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private string _selectedPhotoPath;
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPhotoPath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedPhotoPath);
                bitmap.EndInit();
                EmployeeImage.Source = bitmap;
            }
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