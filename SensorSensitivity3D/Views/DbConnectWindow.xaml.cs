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

namespace SensorSensitivity3D.Views
{
    /// <summary>
    /// Логика взаимодействия для DbConnectWindow.xaml
    /// </summary>
    public partial class DbConnectWindow : Window
    {
        public DbConnectWindow()
        {
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
