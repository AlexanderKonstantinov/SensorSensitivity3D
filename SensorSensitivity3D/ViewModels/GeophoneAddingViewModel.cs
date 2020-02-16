using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.ViewModels.Base;
using System.Collections.Generic;

namespace SensorSensitivity3D.ViewModels
{
    public class GeophoneAddingViewModel : BaseViewModel
    {
        public int GeophoneCount { get; set; }
        public List<Geophone> Geophones { get; set; }


        protected override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
