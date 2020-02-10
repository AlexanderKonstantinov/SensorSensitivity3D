using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Base;
using SensorSensitivity3D.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SensorSensitivity3D.Domain.Models
{
    public class GeophoneModel : NamedEntity, INotifyPropertyChanged
    {
        [XmlIgnore]
        public Geophone OriginalGeophone { get; set; }

        [XmlIgnore]
        public Entity[] Entities { get; private set; } = new Entity[2];

        [XmlIgnore]
        public Entity CenterSphere
        {
            get => Entities[0];
            set => Entities[0] = value;
        }

        [XmlIgnore]
        public Entity SensitivitySphere
        {
            get => Entities[1];
            set => Entities[1] = value;
        }

        [XmlIgnore]
        public string DisplayName => $"{Name} ({HoleNumber})";


        [XmlIgnore]
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


        #region geophone properties

        private string _name;
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

        private int _holeNumber;
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
            }
        }

        private bool _isGood;
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

        private bool _gIsVisible;
        public bool GIsVisible
        {
            get => _gIsVisible;
            set
            {
                _gIsVisible = value;
                OnPropertyChanged(nameof(GIsVisible));
                OnPropertyChanged(nameof(IsChanged));

                CenterSphere.Visible = value;
            }
        }

        private bool _sIsVisible;
        public bool SIsVisible
        {
            get => _sIsVisible;
            set
            {
                _sIsVisible = value;
                OnPropertyChanged(nameof(SIsVisible));
                OnPropertyChanged(nameof(IsChanged));

                SensitivitySphere.Visible = value;
            }
        }

        private string _color = $"#000000";
        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
                OnPropertyChanged(nameof(IsChanged));

                CenterSphere.Color = SensitivitySphere.Color = ColorTranslator.FromHtml(value);
            }
        }

        #endregion
               
        #region constructors

        public GeophoneModel()
        {
            InitEntities();
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

            InitEntities();
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

            SensitivitySphere = source.SensitivitySphere;
            CenterSphere = source.CenterSphere;
        }

        #endregion

        /// <summary>
        /// Создание объектов модели, представляющих геофон и его предел чувствительности
        /// </summary>
        [OnSerialized()]
        public void InitEntities()
        {
            var center = Mesh.CreateSphere(3, 20, 20, Mesh.natureType.Smooth);
            center.Color = ColorTranslator.FromHtml(Color);
            center.ColorMethod = colorMethodType.byEntity;
            center.Translate(X, Y, Z);
            center.Visible = GIsVisible;
            
            CenterSphere = center;
            var sensitivitySphere = Mesh.CreateSphere(R > 0 ? R : 1, 30, 30, Mesh.natureType.Smooth);
            sensitivitySphere.Color = ColorTranslator.FromHtml(Color);
            sensitivitySphere.ColorMethod = colorMethodType.byEntity;
            sensitivitySphere.Translate(X, Y, Z);
            sensitivitySphere.Visible = SIsVisible;
            SensitivitySphere = sensitivitySphere;
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

            InitEntities();

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

        public void AcceptChanges()
        {
            OriginalGeophone.Name = Name;
            OriginalGeophone.HoleNumber = HoleNumber;
            OriginalGeophone.X = X;
            OriginalGeophone.Y = Y;
            OriginalGeophone.Z = Z;
            OriginalGeophone.R = R;
            OriginalGeophone.IsGood = IsGood;
            OriginalGeophone.GIsVisible = GIsVisible;
            OriginalGeophone.SIsVisible = SIsVisible;
            OriginalGeophone.Color = Color;
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

        public static Geophone GeophoneModelToGeophone(GeophoneModel geophoneModel)
            => new Geophone
            {
                Name = geophoneModel.Name,
                HoleNumber = geophoneModel.HoleNumber,
                X = geophoneModel.X,
                Y = geophoneModel.Y,
                Z = geophoneModel.Z,
                R = geophoneModel.R,
                Color = geophoneModel.Color,
                GIsVisible = geophoneModel.GIsVisible,
                SIsVisible = geophoneModel.SIsVisible,
                IsGood = geophoneModel.IsGood,
            };
    }
}
