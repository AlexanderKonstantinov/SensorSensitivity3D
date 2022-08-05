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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SensorSensitivity3D.Views.ControlZones
{
    /// <summary>
    /// Логика взаимодействия для ControlZone.xaml
    /// </summary>
    public partial class ControlZone : UserControl
    {
        public ControlZone()
        {
            InitializeComponent();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
            => ColorEditorPopupControlZonePanel.IsOpen = true;

        private void ColorEditor_MouseLeave(object sender, MouseEventArgs e)
            => ColorEditorPopupControlZonePanel.IsOpen = false;
    }
}
