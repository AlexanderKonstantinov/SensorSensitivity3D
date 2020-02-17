using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.Base;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophonesEditorViewModel : BaseViewModel
    {
        public Action<IEnumerable<GeophoneModel>> OnGeophonesAdding;


        public bool IsFull { get; }
        public string GeophonesColor { get; set; } = "#808080"; // gray
        public int GeophonesSensitivityLimit { get; set; } = 150;

        public int TotalGeophoneCount { get; }

        public IEnumerable<GeophoneModel> Geophones { get; set; }
        
        public GeophonesEditorViewModel() { }

        public GeophonesEditorViewModel(IEnumerable<GeophoneModel> geophones, bool isFull)
        {
            IsFull = isFull;

            Geophones = geophones;

            TotalGeophoneCount = Geophones.Count();
        }

        private RelayCommand _addGeophonesCommand;
        public ICommand AddGeophonesCommand
            => _addGeophonesCommand ??= new RelayCommand(ExecuteAddGeophonesCommand);

        private void ExecuteAddGeophonesCommand(object obj)
        {


            //var selectedGeophones = Geophones
            //    .Where(g => g.IsSelected)
            //    .Select(g => g.GeophoneModel);

            //foreach (var g in selectedGeophones)
            //{
            //    g.Color = GeophonesColor;
            //    g.R = GeophonesSensitivityLimit;
            //}

            //OnGeophonesAdding(selectedGeophones);
        }

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
