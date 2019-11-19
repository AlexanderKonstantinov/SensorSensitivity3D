using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Entities.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Configuration : NamedEntity
    {
        public int ModelId { get; set; }

        public bool SIsVisible { get; set; }

        /// <summary>
        /// Использовать общие параметры для всех геофонов
        /// </summary>
        public bool SUseCommonParams { get; set; }
        public string SColor { get; set; }

        public bool RIsVisible { get; set; }

        /// <summary>
        /// Использовать общие параметры для всех радиусов геофонов
        /// </summary>
        public bool RUseCommonParams { get; set; }
        public string RColor { get; set; }
        
        public virtual ICollection<Sensor> Sensors { get; set; }
        public virtual ICollection<SensorZone> SensorZones { get; set; }

        [ForeignKey(nameof(ModelId))]
        public virtual Drawing Drawing { get; set; }
    }
}
