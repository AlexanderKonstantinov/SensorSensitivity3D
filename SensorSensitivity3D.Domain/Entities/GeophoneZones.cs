using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class GeophoneZones : BaseEntity
    {
        public int GeophoneId { get; set; }
        public int ZoneId { get; set; }
        
        [ForeignKey(nameof(GeophoneId))]
        public virtual Geophone Geophone { get; set; }

        [ForeignKey(nameof(ZoneId))]
        public virtual Zone Zone { get; set; }
    }
}
