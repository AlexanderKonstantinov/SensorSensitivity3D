
using SensorSensitivity3D.Domain.Base.Interfaces;

namespace SensorSensitivity3D.Domain.Models
{
    public class GeometricZone : ICopy<GeometricZone>
    {
        public int XMin { get; set; }
        public int XMax { get; set; }
        public int YMin { get; set; }
        public int YMax { get; set; }
        public int ZMin { get; set; }
        public int ZMax { get; set; }

        public int Volume => (XMax - XMin) * (YMax - YMin) * (ZMax - ZMin);

        public void CopyTo(GeometricZone target)
        {
            target.XMin = XMin;
            target.XMax = XMax;
            target.YMin = YMin;
            target.YMax = YMax;
            target.ZMin = ZMin;
            target.ZMax = ZMax;
        }

        public override bool Equals(object obj)
        {
            var zone = obj as GeometricZone;

            if (zone == this)
                return true;

            if (zone is null ||
                GetHashCode() != zone.GetHashCode())
                return false;

            return
                zone.XMin == XMin &&
                zone.XMax == XMax &&
                zone.YMin == YMin &&
                zone.YMax == YMax &&
                zone.ZMin == ZMax &&
                zone.ZMax == ZMax;
        }

        public override int GetHashCode()
        {
            double hashCode = 811243127;
            hashCode = hashCode * -1521134295 + XMin.GetHashCode();
            hashCode = hashCode * -1521134295 + XMax.GetHashCode();
            hashCode = hashCode * -1521134295 + YMin.GetHashCode();
            hashCode = hashCode * -1521134295 + YMax.GetHashCode();
            hashCode = hashCode * -1521134295 + ZMin.GetHashCode();
            hashCode = hashCode * -1521134295 + ZMax.GetHashCode();
            return (int)hashCode;
        }
    }
}
