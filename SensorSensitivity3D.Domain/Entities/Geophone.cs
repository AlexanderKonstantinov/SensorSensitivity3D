using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SensorSensitivity3D.Domain.Base;
using SensorSensitivity3D.Domain.Base.Interfaces;

namespace SensorSensitivity3D.Domain.Entities
{
    [Serializable]
    public sealed class Geophone : NamedEntity, ICopy<Geophone>
    {       
        public int ConfigId { get; set; }
        public int HoleNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int R { get; set; }
        public string Color { get; set; } = "#000000";
        public bool GIsVisible { get; set; }
        public bool SIsVisible { get; set; }
        public bool IsGood { get; set; }
        

        [ForeignKey(nameof(ConfigId))]
        public Configuration Configuration { get; set; }

        public Geophone() { }
        public Geophone(Geophone source)
        {
            source.CopyTo(this);
        }

        public void CopyTo(Geophone target)
        {
            target.Name = Name;
            target.HoleNumber = HoleNumber;
            target.X = X;
            target.Y = Y;
            target.Z = Z;
            target.R = R;
            target.Color = Color;
            target.GIsVisible = GIsVisible;
            target.SIsVisible = SIsVisible;
            target.IsGood = IsGood;
        }

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