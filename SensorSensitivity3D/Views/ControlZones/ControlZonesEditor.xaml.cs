using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SensorSensitivity3D.Views.ControlZones
{
    /// <summary>
    /// Логика взаимодействия для ControlZonesEditor.xaml
    /// </summary>
    public partial class ControlZonesEditor : Window
    {
        public ControlZonesEditor(string title)
        {
            InitializeComponent();

            this.Title = title;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
            => ColorEditorPopupZonePanel.IsOpen = true;

        private void ColorEditor_MouseLeave(object sender, MouseEventArgs e)
            => ColorEditorPopupZonePanel.IsOpen = false;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectAllButton.IsChecked.Value)
                ZoneList.SelectionHelper.SelectItems(ZoneList.Items);
            else
                ZoneList.SelectionHelper.DeselectItems(ZoneList.Items);
        }

        private void ZoneList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectAllButton.IsChecked = ZoneList.SelectedItems.Count == ZoneList.Items.Count;

            SelectAllButton.Content = SelectAllButton.IsChecked.Value ? "Сбросить все" : "Выбрать все";
        }

        private void ControlZoneEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            ZoneList.SelectionHelper.SelectItems(ZoneList.Items);
        }
    }
}
