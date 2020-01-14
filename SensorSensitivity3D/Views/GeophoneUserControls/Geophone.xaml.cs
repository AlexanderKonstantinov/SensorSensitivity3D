using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SensorSensitivity3D.Views.GeophoneUserControls
{
    /// <summary>
    /// Логика взаимодействия для Geophone.xaml
    /// </summary>
    public partial class Geophone : UserControl
    {
        public Geophone()
        {
            InitializeComponent();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = true;

        private void ColorEditor_MouseLeave(object sender, MouseEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = false;

        private void ColorEditorOk_Click(object sender, RoutedEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = false;
    }
}
