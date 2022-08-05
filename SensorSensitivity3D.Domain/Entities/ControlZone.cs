using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Base;
using SensorSensitivity3D.Domain.Base.Interfaces;
using SensorSensitivity3D.Domain.Models;

namespace SensorSensitivity3D.Domain.Entities
{
    public class ControlZone : NamedEntity, ICopy<ControlZone>
    {
        public int ConfigId { get; set; }
        public string Color { get; set; }
        public bool IsVisible { get; set; }

        public int XMin { get; set; }
        public int XMax { get; set; }
        public int YMin { get; set; }
        public int YMax { get; set; }
        public int ZMin { get; set; }
        public int ZMax { get; set; }

        public bool IsCalculated { get; set; }
        public double SMin { get; set; }
        public double SMax { get; set; }
        public double K { get; set; }

        [ForeignKey(nameof(ConfigId))]
        public Configuration Configuration { get; set; }

        public ControlZone() { }
        public ControlZone(ControlZone source)
        {
            source.CopyTo(this);
        }

        public void CopyTo(ControlZone target)
        {
            target.Name = Name;
            target.Color = Color;
            target.IsVisible = IsVisible;

            target.XMin = XMin;
            target.XMax = XMax;
            target.YMin = YMin;
            target.YMax = YMax;
            target.ZMin = ZMin;
            target.ZMax = ZMax;

            target.IsCalculated = IsCalculated;
            target.SMin = SMin;
            target.SMax = SMax;
            target.K = K;
        }
    }
}
