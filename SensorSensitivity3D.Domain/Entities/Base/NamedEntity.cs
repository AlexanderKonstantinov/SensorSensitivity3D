using SensorSensitivity3D.Domain.Entities.Base.Interfaces;

namespace SensorSensitivity3D.Domain.Entities.Base
{
    public abstract class NamedEntity : BaseEntity, INamedEntity
    {
        public string Name { get; set; }
    }
}
