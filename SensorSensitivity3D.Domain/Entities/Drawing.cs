using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Entities.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Drawing : NamedEntity
    {
        public int ConfigurationId { get; set; }

        public string ModelPath { get; set; }
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double ZMin { get; set; }
        public double ZMax { get; set; }

        [ForeignKey(nameof(ConfigurationId))]
        public virtual Configuration Configuration { get; set; }
        
        public Drawing(string path)
        {
            ModelPath = path;
        }
    }
}
