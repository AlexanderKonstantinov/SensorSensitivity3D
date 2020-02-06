
using devDept.Eyeshot.Entities;

namespace SensorSensitivity3D.Domain.Models
{
    public class Drawing
    {
        public bool IsVisible { get; set; }
        public string Name { get; set; }
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double ZMin { get; set; }
        public double ZMax { get; set; }

        public Entity[] Entities { get; set; }
    }
}
