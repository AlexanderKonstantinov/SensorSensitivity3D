using System.Windows;
using System.Windows.Input;
using devDept.Eyeshot;
using SensorSensitivity3D.DAL;
using SensorSensitivity3D.ViewModels;
using Telerik.Windows.Controls;

namespace SensorSensitivity3D.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            
            InitializeComponent();

            // Указание на путь к БД
            Context.ContextConfiguring(@"C:\Users\Александер\Documents\GitHub\SensorSensitivity3D\SensorSensitivity3D\SS3D.sqlite");


            _viewModel = new MainViewModel(Model);

            DataContext = _viewModel;
            
            //GeophoneTabItem.Content = new Geophones();

            //var factory = new Context.ContextFactory();

            //using (var dataContext = factory.CreateDbContext(null))
            //{
            //    //dataContext.Geophone.Add(new MyEntity { MyColumn = "aaa" });
            //    //dataContext.MyTable.Add(new MyEntity { MyColumn = "bbb" });
            //    //dataContext.MyTable.Add(new MyEntity { MyColumn = "ccc" });

            //    //dataContext.SaveChanges();

            //    //foreach (var cat in dataContext.MyTable.ToList())
            //    //{
            //    //    Console.WriteLine($@"CategoryId= {cat.MyColumn}, CategoryName = {cat.MyColumn}");
            //    //}

            //    Console.ReadLine();
            //}
        }

        private void RightPanel_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IInputElement focusableElement = FocusManagerHelper.GetFocusedElement(this);

            if (focusableElement is RadSplitButton)
                RightPanel.Focus();
        }

        private void NewConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            NewConfigurationPanel.Visibility = Visibility.Visible;
            NewConfigurationButton.Visibility = Visibility.Collapsed;
            NewConfigurationText.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (NewConfigurationPanel.Visibility == Visibility.Visible 
                && !NewConfigurationText.IsMouseOver
                && !AddConfigButton.IsMouseOver)
            {
                NewConfigurationPanel.Visibility = Visibility.Collapsed;
                NewConfigurationButton.Visibility = Visibility.Visible;
            }
        }

        private void SwitchNameConfigFieldVisibility(object sender, RoutedEventArgs e)
        {
            NameConfigField.IsOpen = !NameConfigField.IsOpen;

            if (NameConfigField.IsOpen)
                EditedConfigName.Focus();   
        }

        private void EditedConfigName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (EditedConfigName.CaretIndex == 0)
                EditedConfigName.CaretIndex = EditedConfigName.Text.Length;
        }

        private void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            NewConfigurationPanel.Visibility = Visibility.Collapsed;
            NewConfigurationButton.Visibility = Visibility.Visible;
        }

        private void ModelSpaceLeave(object sender, MouseEventArgs e)
        {
            EntityInfo.Text = null;
        }

        private void Model_SelectionChanged(object sender, Environment.SelectionChangedEventArgs e)
        {
            EntityPopup.IsOpen = false;
            EntityPopup.IsOpen = !string.IsNullOrEmpty(EntityInfo.Text);
        }
    }
}