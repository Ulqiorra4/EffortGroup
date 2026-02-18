using EffortGroup.ApplicationData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EffortGroup.Admin
{
    /// <summary>
    /// Логика взаимодействия для PageAdmin.xaml
    /// </summary>
    public partial class PageAdmin : Page
    {
        public ObservableCollection<ButtonModel> Buttons { get; set; }
        public List<Peripherals> allPeripherals { get; private set; }

        private List<ApplicationData.Employee> allEmployees; // Хранит все записи сотрудников
        private List<Computers> allComputers; // Хранит все записи компьютеров

        public PageAdmin()
        {
            InitializeComponent();
            Buttons = new ObservableCollection<ButtonModel>();
            DataContext = this;


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
        private void Login_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void Login_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Search.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void CompTech_Click(object sender, RoutedEventArgs e)
        {
            LoadComputerDataAsync();
            showingEmployees = false;
            AddComp.Visibility = Visibility.Visible;
            btnAddEmployee.Visibility = Visibility.Collapsed;
        }

        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployeeDataAsync();
            showingEmployees = true;
            btnAddEmployee.Visibility = Visibility.Visible;
            AddComp.Visibility = Visibility.Collapsed;
        }

        private void Per_Click(object sender, RoutedEventArgs e)
        {
            LoadPeripheralDataAsync();
            showingEmployees = false;
            AddComp.Visibility = Visibility.Collapsed; // Изменяем видимость кнопки AddComp
            btnAddEmployee.Visibility = Visibility.Collapsed;  // Скрываем кнопку добавления сотрудника
            AddPer.Visibility = Visibility.Visible; // Показываем кнопку добавления периферии
        }

        public async Task LoadEmployeeDataAsync()
        {
            MessageBox.Show("Начинаю загрузку данных...");
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    var employeeData = await Task.Run(() => {
                        return context.Employee
                            .Select(c => new
                            {
                                IdEmp = c.IdEmp,
                                FullName = c.FullName,
                                Phone = c.Phone,
                                PassNumb = c.PassNumb,
                                PassSeries = c.PassSeries,
                                IdDep = c.IdDep,
                                Post = c.Post,
                                PhotoPath = c.PhotoPath
                            }).ToList();
                    });

                    allEmployees = employeeData.Select(c => new EffortGroup.ApplicationData.Employee
                    {
                        IdEmp = c.IdEmp,
                        FullName = c.FullName,
                        Phone = c.Phone,
                        PassNumb = c.PassNumb,
                        PassSeries = c.PassSeries,
                        IdDep = c.IdDep,
                        Post = c.Post,
                        PhotoPath = c.PhotoPath,
                        IsSelected = false
                    }).ToList();


                    // Выводим отладочную информацию для проверки
                    foreach (var emp in allEmployees)
                    {
                        Console.WriteLine($"Employee Id: {emp.IdEmp}, FullName: {emp.FullName}, PhotoPath: {emp.PhotoPath}");
                    }

                    MessageBox.Show($"Загружено записей: {allEmployees.Count}");

                    CreateButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private bool _showingEmployees; // Теперь это backing field
        public bool showingEmployees
        {
            get { return _showingEmployees; }
            set { _showingEmployees = value; } // Свойство для доступа к полю
        }


        private bool showingPerif = true;

        private void CreateButtons()
        {
            // Очищаем старые кнопки, если они есть.
            ButtonsPanel.Children.Clear();

            if (showingEmployees || !showingPerif && allEmployees != null)
            {
                foreach (var employee in allEmployees)
                {
                    Button button = new Button();
                    button.Tag = employee.IdEmp;
                    button.Click += OpenEmployeeProfile;
                    button.Cursor = Cursors.Hand;

                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Vertical;

                    Image image = new Image();
                    if (!string.IsNullOrEmpty(employee.PhotoPath))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(employee.PhotoPath, UriKind.Absolute);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                    }
                    image.MaxWidth = 40;
                    image.MaxHeight = 40;
                    stackPanel.Children.Add(image);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = employee.FullName;
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.MaxWidth = 40;
                    stackPanel.Children.Add(textBlock);

                    button.Content = stackPanel;
                    ButtonsPanel.Children.Add(button);
                }
            }
             if (!showingEmployees && allComputers != null)
            {
                if (allComputers.Count == 0)
                {
                    MessageBox.Show("Нет данных для отображения компьютеров.");
                    return;
                }

                foreach (var computer in allComputers)
                {
                    Button button = new Button();
                    button.Tag = computer.IdComp;
                    button.Click += OpenComputerProfile;
                    button.Cursor = Cursors.Hand;

                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Vertical;

                    Image image = new Image();
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri("/Images/computer.png", UriKind.Relative);
                    bitmapImage.EndInit();
                    image.Source = bitmapImage;

                    image.MaxWidth = 40;
                    image.MaxHeight = 40;
                    stackPanel.Children.Add(image);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = $"{computer.Brand} {computer.Model}";
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.MaxWidth = 40;
                    stackPanel.Children.Add(textBlock);

                    button.Content = stackPanel;
                    ButtonsPanel.Children.Add(button);
                }
            }
            else if (!showingEmployees && showingPerif && allPeripherals != null)
            {
                
                if (allPeripherals.Count == 0)
                {
                    MessageBox.Show("Нет данных для отображения периферийных устройств.");
                    return;
                }

                foreach (var peripheral in allPeripherals)
                {
                    Button button = new Button();
                    button.Tag = peripheral.IdPer;
                    button.Click += OpenPeripheralProfile;
                    button.Cursor = Cursors.Hand;

                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Vertical;

                    Image image = new Image();
                    if (!string.IsNullOrEmpty(peripheral.PhotoPer))
                    {
                        try
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = new Uri(peripheral.PhotoPer, UriKind.Absolute);
                            bitmapImage.EndInit();
                            image.Source = bitmapImage;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка загрузки фото периферии {peripheral.NamePer}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = new Uri("/Images/Periph.png", UriKind.Relative);
                            bitmapImage.EndInit();
                            image.Source = bitmapImage;
                        }
                    }
                    else
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("/Images/Periph.png", UriKind.Relative);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                    }
                    image.MaxWidth = 40;
                    image.MaxHeight = 40;
                    stackPanel.Children.Add(image);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = $"{peripheral.Type} {peripheral.NamePer}";
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.MaxWidth = 40;
                    stackPanel.Children.Add(textBlock);

                    button.Content = stackPanel;
                    ButtonsPanel.Children.Add(button);
                }
            }
        }
        // Метод для переключения между сотрудниками и компьютерами
        private void ToggleView()
        {
            Console.WriteLine("ToggleEmployeeView вызван."); // Отладочное сообщение
            showingEmployees = !showingEmployees;
            showingPerif = !showingPerif;
            Console.WriteLine($"Состояние showingEmployees изменено на: {showingEmployees}"); // Отладочное сообщение
            CreateButtons(); // Обновляем кнопки после изменения состояния
        }
        private void OpenEmployeeProfile(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int employeeId = (int)button.Tag; // Получаем IdEmp сотрудника из Tag

            // Создаем и открываем окно редактирования
            EditEmployeeWindow editWindow = new EditEmployeeWindow(employeeId);
            editWindow.ShowDialog(); // Используем ShowDialog, чтобы окно было модальным
        }

        private void OpenComputerProfile(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int computerId = (int)button.Tag;

            PageEditComp editWindow = new PageEditComp(computerId);
            NavigationService.Navigate(editWindow);
        }

        private void OpenPeripheralProfile(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int peripheralId = (int)button.Tag; // Получаем IdPer периферийного устройства из Tag

            // Создаем и открываем окно редактирования
            PageEditPer editWindow = new PageEditPer(peripheralId);
            AppFrame.frameMain.Navigate(editWindow);

        }
        

        public async Task LoadComputerDataAsync()
        {
            MessageBox.Show("Начинаю загрузку данных...");
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    var computerData = await Task.Run(() => {
                        return context.Computers
                            .Select(c => new
                            {
                                IdComp = c.IdComp,
                                Brand = c.Brand,
                                Model = c.Model,
                                PhotoComp = c.PhotoComp,
                                IdEmp = c.IdEmp
                            }).ToList();
                    });

                    allComputers = computerData.Select(c => new EffortGroup.ApplicationData.Computers
                    {
                        IdEmp = c.IdEmp,
                        IdComp = c.IdComp,
                        Brand = c.Brand,
                        Model = c.Model,
                        PhotoComp = c.PhotoComp,
                        IsSelected = false
                    }).ToList();


                    // Выводим отладочную информацию для проверки
                    foreach (var comp in allComputers)
                    {
                        Console.WriteLine($"Computer Id: {comp.IdComp}, Brand: {comp.Brand}, Model: {comp.Model}, IdEmp: {comp.IdEmp}");
                    }

                    MessageBox.Show($"Загружено записей: {allComputers.Count}");

                    CreateButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public async Task LoadPeripheralDataAsync()
        {

            MessageBox.Show("Начинаю загрузку данных о периферии...");
            try
            {
                using (var context = new EffortGroupEntities())
                {
                    var peripheralData = await Task.Run(() => {
                        return context.Peripherals
                            .Select(p => new
                            {
                                IdPer = p.IdPer,
                                Type = p.Type,
                                NamePer = p.NamePer,
                                IdComp = p.IdComp,
                                PhotoPer = p.PhotoPer
                            }).ToList();
                    });

                    allPeripherals = peripheralData.Select(p => new EffortGroup.ApplicationData.Peripherals
                    {
                        IdPer = p.IdPer,
                        Type = p.Type,
                        NamePer = p.NamePer,
                        IdComp = p.IdComp,
                        PhotoPer = p.PhotoPer,
                        IsSelected = false
                    }).ToList();

                    // Выводим отладочную информацию для проверки
                    foreach (var periph in allPeripherals)
                    {
                        Console.WriteLine($"Peripheral Id: {periph.IdPer}, Type: {periph.Type}, Name: {periph.NamePer}, IdComp: {periph.IdComp}, PhotoPer: {periph.PhotoPer}");
                    }

                    MessageBox.Show($"Загружено записей о периферии: {allPeripherals.Count}");

                    CreateButtons(); // Вызываем CreateButtons после загрузки данных
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных о периферии: {ex.Message}");
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var pageAddEmp = new PageAddEmp();
            pageAddEmp.OnEmployeeAdded += () => LoadEmployeeDataAsync();
            AppFrame.frameMain.Navigate(new PageAddEmp());
        }

        private void AddComp_Click(object sender, RoutedEventArgs e)
        {
            var pageAddComp = new PageAddComp(0);
            pageAddComp.OnComputerAdded += () => LoadComputerDataAsync();
            AppFrame.frameMain.Navigate(pageAddComp);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterButtons();
        }

        private void FilterButtons()
        {
            string searchText = Search.Text.Trim().ToLower();

            foreach (var child in ButtonsPanel.Children)
            {
                if (child is Button button)
                {
                    StackPanel stackPanel = button.Content as StackPanel;
                    if (stackPanel != null)
                    {
                        TextBlock textBlock = stackPanel.Children.OfType<TextBlock>().FirstOrDefault();
                        if (textBlock != null)
                        {
                            string fullName = textBlock.Text.ToLower();
                            if (fullName == searchText) // Используем точное сравнение
                            {
                                button.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGreen);  // Подсветка для совпадения
                                button.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                button.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightBlue);  // Возвращаем исходный цвет
                                if (string.IsNullOrEmpty(searchText))
                                    button.Visibility = Visibility.Visible; // Если поиск пуст, показываем все кнопки
                                else
                                    button.Visibility = Visibility.Collapsed; // Скрываем несовпавшие
                            }
                        }
                    }
                }
            }

            
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddPer_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new PageAddPer());
        }
    }
}
