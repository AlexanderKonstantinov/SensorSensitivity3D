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

            foreach (var g in selectedGeophones)
            {
                g.Color = GeophonesColor;
                g.R = GeophonesSensitivityLimit;
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
            OnPropertyChanged(nameof(SelectedGeophoneCount));
        }

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
