using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Base;

namespace SensorSensitivity3D.Domain.Entities
{
    [Serializable]
    public class Geophone : NamedEntity
    {       
        public int ConfigId { get; set; }

        [Required]
        public int HoleNumber { get; set; }
        [Required]
        public double X { get; set; }
        [Required]
        public double Y { get; set; }
        [Required]
        public double Z { get; set; }
        [Required]
        public bool IsGood { get; set; }
        [Required]
        public bool GIsVisible { get; set; }
        [Required]
        public bool SIsVisible { get; set; }
        [Required]
        public string Color { get; set; } = "#000000";

        [Required]
        /// <summary>
        /// Предел чувствительности
        /// </summary>
        public int R { get; set; }
        
        public virtual ICollection<Zone> Zones { get; set; }

        [Required]
        [ForeignKey(nameof(ConfigId))]
        public virtual Configuration Configuration { get; set; }

        public override bool Equals(object obj)
        {
            var geophone = obj as Geophone;

            if (geophone == this)
                return true;

            if (geophone is null ||
                GetHashCode() != geophone.GetHashCode())
                return false;

            return
                geophone.Name == Name &&
                geophone.HoleNumber == HoleNumber &&
                geophone.X == X &&
                geophone.Y == Y &&
                geophone.Z == Z &&
                geophone.IsGood == IsGood &&
                geophone.Color == Color &&
                geophone.GIsVisible == GIsVisible &&
                geophone.SIsVisible == SIsVisible &&
                geophone.R == R;
        }

        public override int GetHashCode()
        {
            double hashCode = 811243127;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + HoleNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + IsGood.GetHashCode();
            if (Color != null) hashCode = hashCode * -1521134295 + Color.GetHashCode();
            hashCode = hashCode * -1521134295 + GIsVisible.GetHashCode();
            hashCode = hashCode * -1521134295 + SIsVisible.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            return (int)hashCode;
        }
    }
}