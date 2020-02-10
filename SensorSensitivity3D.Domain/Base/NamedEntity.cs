using SensorSensitivity3D.Domain.Base.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace SensorSensitivity3D.Domain.Base
{
    [Serializable]
    public abstract class NamedEntity : BaseEntity, INamedEntity
    {
        public virtual string Name { get; set; }
    }
}
