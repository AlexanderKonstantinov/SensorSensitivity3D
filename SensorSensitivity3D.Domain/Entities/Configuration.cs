
using SensorSensitivity3D.Domain.Base;
using System.ComponentModel;

namespace SensorSensitivity3D.Domain.Entities
{
    public class Configuration : NamedEntity, INotifyPropertyChanged
    {
        private string _name;
        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _drawingIsVisible;
        public bool DrawingIsVisible
        {
            get => _drawingIsVisible;
            set
            {
                _drawingIsVisible = value;
                OnPropertyChanged(nameof(DrawingIsVisible));
            }
        }

        private string _substratePath;
        public string SubstratePath
        {
            get => _substratePath;
            set
            {
                _substratePath = value;
                OnPropertyChanged(nameof(SubstratePath));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
