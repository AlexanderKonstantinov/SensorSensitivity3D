using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SensorSensitivity3D.Views.GeophoneUserControls
{
    /// <summary>
    /// Логика взаимодействия для Geophones.xaml
    /// </summary>
    public partial class Geophones : UserControl
    {
        public Geophones()
        {
            InitializeComponent();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e) 
            => ColorEditorPopup.IsOpen = true;

        private void ColorEditor_MouseLeave(object sender, MouseEventArgs e) 
            => ColorEditorPopup.IsOpen = false;
        

        //private void GeophonePanelShow(object sender, RoutedEventArgs e)
        //=> GeophonePanel.Visibility = Visibility.Visible;
                   

        private void GeophonesPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            => GeophonePanel.Visibility = GeophonePanel.Visibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;

    }
}
