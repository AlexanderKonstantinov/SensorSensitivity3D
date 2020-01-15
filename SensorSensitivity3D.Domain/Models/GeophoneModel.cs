using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Base;
using SensorSensitivity3D.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace SensorSensitivity3D.Domain.Models
{
    [Serializable]
    public class GeophoneModel : NamedEntity, INotifyPropertyChanged
    {
        public Geophone OriginalGeophone { get; set; }

        public Entity GeophoneEntity { get; set; }
        public Entity GeophoneSphereEntity { get; set; }

        public string DisplayName => $"{Name} ({HoleNumber})";

        public string _name;
        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public int _holeNumber;
        public int HoleNumber
        {
            get => _holeNumber;
            set
            {
                _holeNumber = value;
                OnPropertyChanged(nameof(HoleNumber));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }


        public bool _isChanged;
        public bool IsChanged
        {
            get => _isChanged;
            set
            {
                _isChanged = value;
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        public bool _isGood;
        public bool IsGood
        {
            get => _isGood;
            set
            {
                _isGood = value;
                OnPropertyChanged(nameof(IsGood));
                IsChanged = CheckGeophoneChanging();
            }
        }

        public bool _gIsVisible;
        public bool GIsVisible
        {
            get => _gIsVisible;
            set
            {
                _gIsVisible = value;
                OnPropertyChanged(nameof(GIsVisible));
                IsChanged = CheckGeophoneChanging();

                GeophoneEntity.Visible = value;
            }
        }

        public bool _sIsVisible;
        public bool SIsVisible
        {
            get => _sIsVisible;
            set
            {
                _sIsVisible = value;
                OnPropertyChanged(nameof(SIsVisible));
                IsChanged = CheckGeophoneChanging();

                GeophoneSphereEntity.Visible = value;
            }
        }

        public string _color;
        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
                IsChanged = CheckGeophoneChanging();

                GeophoneSphereEntity.Color = ColorTranslator.FromHtml(value);
            }
        }

        public void ResetGeophoneSettings()
        {
            SIsVisible = OriginalGeophone.SIsVisible;
            GIsVisible = OriginalGeophone.GIsVisible;
            IsGood = OriginalGeophone.IsGood;
            Color = OriginalGeophone.Color;
            IsChanged = false;
        }

        /// <summary>
        /// Предел чувствительности
        /// </summary>
        public int R { get; set; }

        public GeophoneModel() { }
            
        public GeophoneModel(Geophone g)
        {
            OriginalGeophone = g;
            _name = g.Name;
            _holeNumber = g.HoleNumber;
            X = g.X;
            Y = g.Y;
            Z = g.Z;
            _isGood = g.IsGood;
            _gIsVisible = g.GIsVisible;
            _sIsVisible = g.SIsVisible;
            _color = g.Color;
            R = g.R;
        }

        public override string ToString()
            => $"{DisplayName}\nX: {X}\nY: {Y}\nZ: {Z}\nR: {R}";

        private bool CheckGeophoneChanging() =>
            !(OriginalGeophone is null) &&
            (OriginalGeophone.SIsVisible != SIsVisible ||
            OriginalGeophone.GIsVisible != GIsVisible ||
            OriginalGeophone.IsGood != IsGood ||
            OriginalGeophone.Color != Color);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
        {
            var geophone = obj as GeophoneModel;

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
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            hashCode = hashCode * -1521134295 + GIsVisible.GetHashCode();
            hashCode = hashCode * -1521134295 + SIsVisible.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            return (int)hashCode;
        }
    }
}
