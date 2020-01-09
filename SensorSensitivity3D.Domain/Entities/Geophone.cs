using System;
using System.Collections.Generic;
using SensorSensitivity3D.Domain.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    [Serializable]
    public class Geophone : NamedEntity
    {       

        public int HoleNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool IsGood { get; set; }
        public bool GIsVisible { get; set; }
        public bool SIsVisible { get; set; }
        public string Color { get; set; }        

        /// <summary>
        /// Предел чувствительности
        /// </summary>
        public int R { get; set; }
        
        public virtual ICollection<GeophoneZones> GeophoneZones { get; set; }
    }
}