using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Base.Interfaces;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Domain.Models
{
    public class ZoneModel : INotifyPropertyChanged
    {
        //public List<Geophone> Geophones { get; set; }
        public Entity Body { get; set; }
        public HashSet<DomainCub> Domains { get; set; }

        public int GroupNumber { get; set; }

        private bool _visible;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                if (Body != null)
                    Body.Visible = value;

                OnPropertyChanged(nameof(Visible));
            }
        }

        public string Color { get; set; }

        //public string DisplayName 
        //    => string.Join(", ", Geophones.Select(g => $"{g.Name} ({g.HoleNumber})"));

        public string DisplayName => $"Зона {GroupNumber + 1}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
