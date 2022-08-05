using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.Base;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophonesEditorViewModel : BaseViewModel
    {
        public Action<IEnumerable<GeophoneModel>> OnSelectionGeophones;
        
        public bool IsFull { get; }
        public string GeophonesColor { get; set; } = "#808080"; // gray
        public int GeophonesSensitivityLimit { get; set; } = 150;

        public int TotalGeophoneCount { get; }
        public int SelectedGeophoneCount => Geophones?.Count(g => g.IsSelected) ?? 0;

        private SelectableGeophone _lastSelectedGeophone;
        public SelectableGeophone LastSelectedGeophone
        {
            get => _lastSelectedGeophone;
            set
            {
                if (value is null)
                    _lastSelectedGeophone.IsSelected = false;
                else
                {
                    _lastSelectedGeophone = value;
                    _lastSelectedGeophone.IsSelected = true;
                }
                OnPropertyChanged(nameof(SelectedGeophoneCount));
            }
        }
        public ObservableCollection<SelectableGeophone> Geophones { get; set; }
        
        public GeophonesEditorViewModel() { }

        /// <summary>
        /// Конструктор окна со списком добавляемых геофонов
        /// </summary>
        /// <param name="geophones">Все загруженные геофоны</param>
        /// <param name="isFull">Указание на частичную или полную инициализацию свойств геофона</param>
        public GeophonesEditorViewModel(IEnumerable<GeophoneModel> geophones, bool isFull)
        {
            IsFull = isFull;

            Geophones = new ObservableCollection<SelectableGeophone>(
                geophones.Select(g => new SelectableGeophone {GeophoneModel = g, IsSelected = true}));
            
            TotalGeophoneCount = Geophones.Count();
        }

        private RelayCommand _addGeophonesCommand;
        public ICommand AddGeophonesCommand
            => _addGeophonesCommand ??= new RelayCommand(ExecuteAddGeophonesCommand, CanExecuteAddGeophonesCommand);

        private void ExecuteAddGeophonesCommand(object obj)
        {
            var selectedGeophones = Geophones
                .Where(g => g.IsSelected)
                .Select(g => g.GeophoneModel);

            if (!IsFull)
            {
                foreach (var g in selectedGeophones)
                {
                    g.Color = GeophonesColor;
                    g.R = GeophonesSensitivityLimit;
                }
            }            

            OnSelectionGeophones?.Invoke(selectedGeophones);
        }

        private bool CanExecuteAddGeophonesCommand(object o)
            => SelectedGeophoneCount > 0;


        private RelayCommand _selectGeophoneCommand;
        public ICommand SelectGeophoneCommand
            => _selectGeophoneCommand ??= new RelayCommand(ExecuteSelectGeophoneCommand);

        private void ExecuteSelectGeophoneCommand(object obj)
        {
            var geophone = obj as SelectableGeophone;
            geophone.IsSelected = !geophone.IsSelected;

            OnPropertyChanged(nameof(SelectedGeophoneCount));
        }

        private RelayCommand _selectAllGeophoneCommand;
        public ICommand SelectAllGeophoneCommand
            => _selectAllGeophoneCommand ??= new RelayCommand(ExecuteAllSelectGeophoneCommand);

        private void ExecuteAllSelectGeophoneCommand(object obj)
        {
            var isSelected = (bool) obj;

            foreach (var g in Geophones)
                g.IsSelected = isSelected;

            OnPropertyChanged(nameof(SelectedGeophoneCount));
        }

        protected override void OnDispose()
        {
            Geophones?.Clear();
        }
    }
}
