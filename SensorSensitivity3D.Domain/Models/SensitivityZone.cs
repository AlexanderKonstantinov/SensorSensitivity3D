using System;
using System.Collections.Generic;
using System.Linq;
using devDept.Eyeshot.Entities;

namespace SensorSensitivity3D.Domain.Models
{
    public class SensitivityZone
    {
        public Entity Body { get; set; }


        public int GroupNumber { get; set; }
        public HashSet<SensitivityDomain> Domains { get; set; }

        public int Volume
        {
            get
            {
                if (Domains.Count == 0)
                    return 0;

                var twoDomains = Domains.Take(2).ToList();
                var size = Math.Abs(twoDomains[0].X - twoDomains[1].X);

                return Domains.Count * size * size * size;
            }
        }

        public double SMin => Domains.Min(d => d.Sensitivity);
        public double SMax => Domains.Max(d => d.Sensitivity);
    }
}
