using System.Collections.Generic;
using SensorSensitivity3D.Domain.Entities.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Sensor : BaseEntity
    {


        public int HoleNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool IsActive { get; set; }
        public string SColor { get; set; }
        public bool SIsVisible { get; set; }

        /// <summary>
        /// Радиус чувствительности
        /// </summary>
        public int R { get; set; }
        public string RColor { get; set; }
        public bool RIsVisible { get; set; }
        
        public virtual ICollection<SensorZone> SensorZones { get; set; }
    }
}