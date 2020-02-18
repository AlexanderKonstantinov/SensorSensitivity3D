
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SensorSensitivity3D.Domain.Models
{
    public class SelectableGeophone : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GeophoneModel GeophoneModel { get; set; }

        public bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
