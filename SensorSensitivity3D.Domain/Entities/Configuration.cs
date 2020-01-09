using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Configuration : NamedEntity
    {
        public int ModelId { get; set; }
        
        public virtual IList<Geophone> Geophones { get; set; }
        public virtual IList<GeophoneZones> GeophoneZones { get; set; }

        [ForeignKey(nameof(ModelId))]
        public virtual Drawing Drawing { get; set; }
    }
}
