
namespace SensorSensitivity3D.Domain.Models
{
    public class SensitivityDomain
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int GeophoneCount { get; set; }
        public double Sensitivity { get; set; }
        public int GroupNumber { get; set; } 
    }
}
