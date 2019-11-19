﻿using System.Collections.Generic;
using SensorSensitivity3D.Domain.Entities.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Zone : BaseEntity
    {
        public string Color { get; set; }
        public bool IsVisible { get; set; }

        public virtual ICollection<SensorZone> SensorZones { get; set; }
    }
}
