
using SensorSensitivity3D.Domain.Base;
using SensorSensitivity3D.Domain.Base.Interfaces;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Configuration : NamedEntity, ICopy<Configuration>
    {
        public bool DrawingIsVisible { get; set; }
        public string SubstratePath { get; set; }

        public void CopyTo(Configuration target)
        {
            target.DrawingIsVisible = DrawingIsVisible;
            target.SubstratePath = SubstratePath;
        }
    }
}
