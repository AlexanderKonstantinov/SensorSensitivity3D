using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Drawing : NamedEntity
    {
        public int ConfigId { get; set; }

        public string Path { get; set; }

        [Required]
        public double XMin { get; set; }
        [Required]
        public double XMax { get; set; }
        [Required]
        public double YMin { get; set; }
        [Required]
        public double YMax { get; set; }
        [Required]
        public double ZMin { get; set; }
        [Required]
        public double ZMax { get; set; }

        [Required]
        [ForeignKey(nameof(ConfigId))]
        public virtual Configuration Configuration { get; set; }

        public Drawing()
        {
            
        }

        public Drawing(string path)
        {
            Path = path;
        }
    }
}
