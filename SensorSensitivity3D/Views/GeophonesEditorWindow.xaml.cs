using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SensorSensitivity3D.Views
{
    /// <summary>
    /// Логика взаимодействия для GeophonesEditorWindow.xaml
    /// </summary>
    public partial class GeophonesEditorWindow : Window
    {
        public GeophonesEditorWindow(string title)
        {
            InitializeComponent();

            this.Title = title;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = true;

        private void ColorEditor_MouseLeave(object sender, MouseEventArgs e)
            => ColorEditorPopupGeophonePanel.IsOpen = false;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectAllButton.IsChecked.Value)
                GeophoneList.SelectionHelper.SelectItems(GeophoneList.Items);
            else
                GeophoneList.SelectionHelper.DeselectItems(GeophoneList.Items);
        }

        private void GeophoneList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectAllButton.IsChecked = GeophoneList.SelectedItems.Count == GeophoneList.Items.Count;
           
            SelectAllButton.Content = SelectAllButton.IsChecked.Value ? "Сбросить все" : "Выбрать все";
        }

        private void GeophonesEditorWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            GeophoneList.SelectionHelper.SelectItems(GeophoneList.Items);
        }
    }
}
