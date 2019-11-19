using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Entities.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class SensorZone : BaseEntity
    {
        public int SensorId { get; set; }
        public int ZoneId { get; set; }
        
        [ForeignKey(nameof(SensorId))]
        public virtual Sensor Sensor { get; set; }

        [ForeignKey(nameof(ZoneId))]
        public virtual Zone Zone { get; set; }
    }
}
