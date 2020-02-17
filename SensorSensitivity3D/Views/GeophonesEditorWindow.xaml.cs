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

            SelectedGeophoneCount.Text = " 0";
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = true;

        private void ColorEditor_MouseLeave(object sender, MouseEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = false;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RadListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var count = GeophoneList.SelectedItems.Count;

            SelectedGeophoneCount.Text = $" {count}";
            AddButton.IsEnabled = count > 0;
        }
    }
}
