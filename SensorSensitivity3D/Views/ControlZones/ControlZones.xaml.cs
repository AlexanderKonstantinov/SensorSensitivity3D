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
    /// Логика взаимодействия для ControlZones.xaml
    /// </summary>
    public partial class ControlZones : UserControl
    {
        public ControlZones()
        {
            InitializeComponent();
        }

        private void ZoneColorButton_Click(object sender, RoutedEventArgs e)
            => ZoneColorEditorPopup.IsOpen = true;

        private void ZoneColorEditor_MouseLeave(object sender, MouseEventArgs e)
            => ZoneColorEditorPopup.IsOpen = false;

        private void ControlZonePanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        => ControlZoneEditor.Visibility = ControlZoneEditor.Visibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
}
