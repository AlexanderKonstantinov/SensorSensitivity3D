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
        
        
        public bool IsChanged 
            => !(OriginalGeophone is null) &&
               (OriginalGeophone.SIsVisible != SIsVisible ||
                OriginalGeophone.GIsVisible != GIsVisible ||
                OriginalGeophone.IsGood != IsGood ||
                OriginalGeophone.Color != Color ||
                OriginalGeophone.Name != Name ||
                OriginalGeophone.HoleNumber != HoleNumber ||
                OriginalGeophone.X != X ||
                OriginalGeophone.Y != Y ||
                OriginalGeophone.Z != Z ||
                OriginalGeophone.R != R
               );


        public string _name;
        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(IsChanged));
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
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        private double _x;
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(IsChanged));

                GeophoneEntity = CreateGeophoneEntity(this);
                GeophoneSphereEntity = CreateGeophoneSphereEntity(this);
            }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(IsChanged));

                GeophoneEntity = CreateGeophoneEntity(this);
                GeophoneSphereEntity = CreateGeophoneSphereEntity(this);
            }
        }

        private double _z;
        public double Z
        {
            get => _z;
            set
            {
                _z = value;
                OnPropertyChanged(nameof(Z));
                OnPropertyChanged(nameof(IsChanged));

                GeophoneEntity = CreateGeophoneEntity(this);
                GeophoneSphereEntity = CreateGeophoneSphereEntity(this);
            }
        }

        private int _r;
        /// <summary>
        /// Предел чувствительности
        /// </summary>
        public int R
        {
            get => _r;
            set
            {
                _r = value;
                OnPropertyChanged(nameof(R));
                OnPropertyChanged(nameof(IsChanged));

                GeophoneEntity = CreateGeophoneEntity(this);
                GeophoneSphereEntity = CreateGeophoneSphereEntity(this);
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
                OnPropertyChanged(nameof(IsChanged));
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
                OnPropertyChanged(nameof(IsChanged));

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
                OnPropertyChanged(nameof(IsChanged));

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
                OnPropertyChanged(nameof(IsChanged));

                GeophoneSphereEntity.Color = ColorTranslator.FromHtml(value);
            }
        }

        public GeophoneModel()
        {
            GeophoneEntity = CreateGeophoneEntity(this);
            GeophoneSphereEntity = CreateGeophoneEntity(this);
        }

        public GeophoneModel(Geophone source)
        {
            OriginalGeophone = source;

            _name = source.Name;
            _holeNumber = source.HoleNumber;
            _x = source.X;
            _y = source.Y;
            _z = source.Z;
            _r = source.R;
            _color = source.Color;
            _gIsVisible = source.GIsVisible;
            _sIsVisible = source.SIsVisible;
            _isGood = source.IsGood;

            GeophoneEntity = CreateGeophoneEntity(this);
            GeophoneSphereEntity = CreateGeophoneSphereEntity(this);
        }

        public GeophoneModel(GeophoneModel source)
        {
            OriginalGeophone = new Geophone(source.OriginalGeophone);

            _name = source.Name;
            _holeNumber = source.HoleNumber;
            _x = source.X;
            _y = source.Y;
            _z = source.Z;
            _r = source.R;
            _color = source.Color;
            _gIsVisible = source.GIsVisible;
            _sIsVisible = source.SIsVisible;
            _isGood = source.IsGood;

            GeophoneEntity = CreateGeophoneEntity(this);
            GeophoneSphereEntity = CreateGeophoneEntity(this);
        }

        /// <summary>
        /// Сброс к последним сохраненным настройкам
        /// </summary>
        public void ResetGeophoneSettings()
        {
            _name = OriginalGeophone.Name;
            _holeNumber = OriginalGeophone.HoleNumber;
            _x = OriginalGeophone.X;
            _y = OriginalGeophone.Y;
            _z = OriginalGeophone.Z;
            _r = OriginalGeophone.R;
            _isGood = OriginalGeophone.IsGood;
            _gIsVisible = OriginalGeophone.GIsVisible;
            _sIsVisible = OriginalGeophone.SIsVisible;
            _color = OriginalGeophone.Color;

            GeophoneEntity = CreateGeophoneEntity(this);
            GeophoneSphereEntity = CreateGeophoneSphereEntity(this);

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(HoleNumber));
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
            OnPropertyChanged(nameof(R));
            OnPropertyChanged(nameof(IsGood));
            OnPropertyChanged(nameof(GIsVisible));
            OnPropertyChanged(nameof(SIsVisible));
            OnPropertyChanged(nameof(Color));

            OnPropertyChanged(nameof(IsChanged));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
            => $"{DisplayName}\nX: {X}\nY: {Y}\nZ: {Z}\nR: {R}";

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
        

        /// <summary>
        /// Создание сферы, обозначающей геофон
        /// </summary>
        /// <param name="geophone">геофон с параметрами</param>
        /// <returns></returns>
        private static Entity CreateGeophoneEntity(GeophoneModel geophone)
        {
            var mesh = Mesh.CreateSphere(3, 20, 20, Mesh.natureType.Smooth);
            mesh.Color = ColorTranslator.FromHtml(geophone.Color);
            mesh.ColorMethod = colorMethodType.byEntity;
            mesh.Translate(geophone.X, geophone.Y, geophone.Z);
            mesh.Visible = geophone.GIsVisible;

            return mesh;
        }

        /// <summary>
        /// Создание сферы, обозначающей зону чувствительности геофона
        /// </summary>
        /// <param name="geophone">геофон с параметрами</param>
        /// <returns></returns>
        private static Entity CreateGeophoneSphereEntity(GeophoneModel geophone)
        {
            var mesh = Mesh.CreateSphere(geophone.R > 0 ? geophone.R : 1, 30, 30, Mesh.natureType.Smooth);
            mesh.Color = ColorTranslator.FromHtml(geophone.Color);
            mesh.ColorMethod = colorMethodType.byEntity;
            mesh.Translate(geophone.X, geophone.Y, geophone.Z);
            mesh.Visible = geophone.SIsVisible;

            return mesh;
        }
    }
}
