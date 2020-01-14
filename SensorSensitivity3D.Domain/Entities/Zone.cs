using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SensorSensitivity3D.Domain.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Zone : BaseEntity
    {
        [Required]
        public string Color { get; set; }
        [Required]
        public bool IsVisible { get; set; }

        [Required]
        public virtual ICollection<Geophone> Geophones { get; set; }
    }
}
