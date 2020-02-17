using System.Windows;
using System.Windows.Input;

namespace SensorSensitivity3D.Views
{
    /// <summary>
    /// Логика взаимодействия для GeophonesEditorWindow.xaml
    /// </summary>
    public partial class GeophonesEditorWindow : Window
    {
        public GeophonesEditorWindow()
        {
            InitializeComponent();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = true;

        private void ColorEditor_MouseLeave(object sender, MouseEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = false;
    }
}
