using System.Collections.Generic;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.ViewModels.Base;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophonesEditorViewModel : BaseViewModel
    {
        public string GeophonesColor { get; set; } = "#808080"; // gray
        public int GeophonesSensitivityLimit { get; set; }

        public int TotalGeophoneCount { get; set; }
        public int SelectedGeophoneCount { get; set; }
        public List<Geophone> Geophones { get; set; }


        protected override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
