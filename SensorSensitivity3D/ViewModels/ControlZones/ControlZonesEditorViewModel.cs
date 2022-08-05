using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.Base;

namespace SensorSensitivity3D.ViewModels.ControlZones
{
    public class ControlZonesEditorViewModel : BaseViewModel
    {
        public Action<IEnumerable<ControlZoneModel>> OnSelectionZones;

        public bool IsFull { get; }
        public string ZonesColor { get; set; } = "#808080"; // gray

        public int TotalZoneCount { get; }
        public int SelectedZoneCount => Zones?.Count(z => z.IsSelected) ?? 0;

        private SelectableControlZone _lastSelectedZone;
        public SelectableControlZone LastSelectedZone
        {
            get => _lastSelectedZone;
            set
            {
                if (value is null)
                    _lastSelectedZone.IsSelected = false;
                else
                {
                    _lastSelectedZone = value;
                    _lastSelectedZone.IsSelected = true;
                }
                OnPropertyChanged(nameof(SelectedZoneCount));
            }
        }
        public ObservableCollection<SelectableControlZone> Zones { get; set; }

        public ControlZonesEditorViewModel() { }

        /// <summary>
        /// Конструктор окна со списком пространственных зон
        /// </summary>
        /// <param name="zones">Все загруженные пространственные зоны</param>
        /// <param name="isFull">Указание на частичную или полную инициализацию свойств пространственной зоны</param>
        public ControlZonesEditorViewModel(IEnumerable<ControlZoneModel> zones, bool isFull)
        {
            IsFull = isFull;

            Zones = new ObservableCollection<SelectableControlZone>(
                zones.Select(z => new SelectableControlZone
                {
                    ControlZoneModel = z, IsSelected = true
                }));

            TotalZoneCount = Zones.Count();
        }

        private RelayCommand _addZonesCommand;
        public ICommand AddZonesCommand
            => _addZonesCommand ??= new RelayCommand(ExecuteAddZonesCommand, CanExecuteAddZonesCommand);

        private void ExecuteAddZonesCommand(object obj)
        {
            var selectedZones = Zones
                .Where(z => z.IsSelected)
                .Select(z => z.ControlZoneModel);

            if (!IsFull)
                foreach (var z in selectedZones)
                    z.Color = ZonesColor;

            OnSelectionZones?.Invoke(selectedZones);
        }

        private bool CanExecuteAddZonesCommand(object o)
            => SelectedZoneCount > 0;


        private RelayCommand _selectZoneCommand;
        public ICommand SelectZoneCommand
            => _selectZoneCommand ??= new RelayCommand(ExecuteSelectZoneCommand);

        private void ExecuteSelectZoneCommand(object obj)
        {
            var zone = obj as SelectableControlZone;
            zone.IsSelected = !zone.IsSelected;

            OnPropertyChanged(nameof(SelectedZoneCount));
        }

        private RelayCommand _selectAllZonesCommand;
        public ICommand SelectAllZonesCommand
            => _selectAllZonesCommand ??= new RelayCommand(ExecuteAllSelectZonesCommand);

        private void ExecuteAllSelectZonesCommand(object obj)
        {
            var isSelected = (bool)obj;

            foreach (var z in Zones)
                z.IsSelected = isSelected;

            OnPropertyChanged(nameof(SelectedZoneCount));
        }

        protected override void OnDispose()
        {
            Zones?.Clear();
        }
    }
}
