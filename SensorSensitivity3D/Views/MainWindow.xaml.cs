using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using devDept.Eyeshot;
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

            _viewModel = new MainViewModel(Model);
            DataContext = _viewModel;
            
            //var factory = new Context.ContextFactory();

            //using (var dataContext = factory.CreateDbContext(null))
            //{
            //    //dataContext.Sensors.Add(new MyEntity { MyColumn = "aaa" });
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}