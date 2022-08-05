using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Base;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Domain.Models
{
    public class ControlZoneModel : NamedEntity, INotifyPropertyChanged
    {
        public Entity Entity { get; set; }

        public ControlZone SavedZone { get; set; }

        public bool IsChanged
            => !Equals(SavedZone); 

        public bool IsGeometricZoneChanged
           => !(SavedZone is null) &&
              SavedZone.XMin != XMin ||
              SavedZone.XMax != XMax ||
              SavedZone.YMin != YMin ||
              SavedZone.YMax != YMax ||
              SavedZone.ZMin != ZMin ||
              SavedZone.ZMax != ZMax;

        private bool _isCalculated;
        public bool IsCalculated
        {
            get => _isCalculated;
            set
            {
                _isCalculated = value;
                OnPropertyChanged(nameof(IsCalculated));
            }
        }

        #region control zone entity properties

        public GeometricZone GeometricZone { get; set; }

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

        private string _color = $"#808080"; // gray
        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
                Entity.Color = ColorTranslator.FromHtml(value);
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
                Entity.Visible = value;
            }
        }
        
        public int XMin
        {
            get => GeometricZone.XMin;
            set
            {
                if (value > XMax)
                {
                    GeometricZone.XMin = XMax;
                    GeometricZone.XMax = value;
                    OnPropertyChanged(nameof(XMax));
                }
                else
                    GeometricZone.XMin = value;

                OnPropertyChanged(nameof(XMin));
                OnPropertyChanged(nameof(IsChanged));
                OnPropertyChanged(nameof(IsGeometricZoneChanged));
            }
        }

        public int XMax
        {
            get => GeometricZone.XMax;
            set
            {
                if (value < XMin)
                {
                    GeometricZone.XMax = XMin;
                    GeometricZone.XMin = value;
                    OnPropertyChanged(nameof(XMin));
                }
                else
                    GeometricZone.XMax = value;

                OnPropertyChanged(nameof(XMax));
                OnPropertyChanged(nameof(IsChanged));
                OnPropertyChanged(nameof(IsGeometricZoneChanged));
            }
        }

        public int YMin
        {
            get => GeometricZone.YMin;
            set
            {
                if (value > YMax)
                {
                    GeometricZone.YMin = YMax;
                    GeometricZone.YMax = value;
                    OnPropertyChanged(nameof(YMax));
                }
                else
                    GeometricZone.YMin = value;

                OnPropertyChanged(nameof(YMin));
                OnPropertyChanged(nameof(IsChanged));
                OnPropertyChanged(nameof(IsGeometricZoneChanged));
            }
        }

        public int YMax
        {
            get => GeometricZone.YMax;
            set
            {
                if (value < YMin)
                {
                    GeometricZone.YMax = YMin;
                    GeometricZone.YMin = value;
                    OnPropertyChanged(nameof(YMin));
                }
                else
                    GeometricZone.YMax = value;

                OnPropertyChanged(nameof(YMax));
                OnPropertyChanged(nameof(IsChanged));
                OnPropertyChanged(nameof(IsGeometricZoneChanged));
            }
        }

        public int ZMin
        {
            get => GeometricZone.ZMin;
            set
            {
                if (value > ZMax)
                {
                    GeometricZone.ZMin = ZMax;
                    GeometricZone.ZMax = value;
                    OnPropertyChanged(nameof(ZMax));
                }
                else
                    GeometricZone.ZMin = value;

                OnPropertyChanged(nameof(ZMin));
                OnPropertyChanged(nameof(IsChanged));
                OnPropertyChanged(nameof(IsGeometricZoneChanged));
            }
        }

        public int ZMax
        {
            get => GeometricZone.ZMax;
            set
            {
                if (value < ZMin)
                {
                    GeometricZone.ZMax = ZMin;
                    GeometricZone.ZMin = value;
                    OnPropertyChanged(nameof(ZMin));
                }
                else
                    GeometricZone.ZMax = value;

                OnPropertyChanged(nameof(ZMax));
                OnPropertyChanged(nameof(IsChanged));
                OnPropertyChanged(nameof(IsGeometricZoneChanged));
            }
        }
        
        public double SMin { get; set; }
        public double SMax { get; set; }
        public double K { get; set; }

        #endregion

        #region constructors

        public ControlZoneModel()
        {
            GeometricZone = new GeometricZone();

            InitEntity();
        }

        public ControlZoneModel(ControlZone source)
        {
            SavedZone = source;

            _name = source.Name;
            _color = source.Color;
            _isVisible = source.IsVisible;

            GeometricZone = new GeometricZone();

            GeometricZone.XMin = source.XMin;
            GeometricZone.XMax = source.XMax;
            GeometricZone.YMin = source.YMin;
            GeometricZone.YMax = source.YMax;
            GeometricZone.ZMin = source.ZMin;
            GeometricZone.ZMax = source.ZMax;

            SMin = source.SMin;
            SMax = source.SMax;
            K = source.K;

            InitEntity();
        }

        public ControlZoneModel(ControlZoneModel source)
        {
            SavedZone = new ControlZone(source.SavedZone);

            _name = source.Name;
            _color = source.Color;
            _isVisible = source.IsVisible;

            source.GeometricZone.CopyTo(GeometricZone);

            SMin = source.SMin;
            SMax = source.SMax;
            K = source.K;

            Entity = source.Entity;
        }

        #endregion

        /// <summary>
        /// Создание объекта модели, представляющего контролируемую зону
        /// </summary>
        public void InitEntity()
        {
            // default entity
            if (GeometricZone.Volume <= 0)
            {
                Entity = Solid.CreateBox(1, 1, 1);
                return;
            }

            var box = Solid.CreateBox(XMax - XMin, YMax - YMin, ZMax - ZMin);
            box.Color = ColorTranslator.FromHtml(Color);
            box.ColorMethod = colorMethodType.byEntity;
            box.Translate(XMin, YMin, ZMin);
            box.Visible = IsVisible;
            Entity = box;
        }

        /// <summary>
        /// Сброс к последним сохраненным настройкам
        /// </summary>
        public void ResetControlZoneSettings()
        {
            Name = SavedZone.Name;
            Color = SavedZone.Color;
            IsVisible = SavedZone.IsVisible;

            XMin = SavedZone.XMin;
            XMax = SavedZone.XMax;
            YMin = SavedZone.YMin;
            YMax = SavedZone.YMax;
            ZMin = SavedZone.ZMin;
            ZMax = SavedZone.ZMax;

            SMin = SavedZone.SMin;
            SMax = SavedZone.SMax;
            K = SavedZone.K;

            InitEntity();
        }

        /// <summary>
        /// Принять изменения зон
        /// Запускается после редактирования или расчета зоны
        /// </summary>
        public void AcceptChanges()
        {
            SavedZone.Name = Name;
            SavedZone.Color = Color;
            SavedZone.IsVisible = IsVisible;

            SavedZone.XMin = XMin;
            SavedZone.XMax = XMax;
            SavedZone.YMin = YMin;
            SavedZone.YMax = YMax;
            SavedZone.ZMin = ZMin;
            SavedZone.ZMax = ZMax;

            SavedZone.SMin = SMin;
            SavedZone.SMax = SMax;
            SavedZone.K = K;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            var calcInfo = K > 0
                ? $"{SMin:f2} < Sensitivity < {SMax:f2}\nK: {K}"
                : "";

            return $"{Name}\n" +
                   $"{XMin} < X < {XMax}\n" +
                   $"{YMin} < Y < {YMax}\n" +
                   $"{ZMin} < Z < {ZMax}\n" +
                   $"Объем: {GeometricZone.Volume}\n" +
                   $"{calcInfo}";
        }

        public bool Equals(ControlZone zone)
            => zone != null &&
               zone.Name == Name &&
               zone.Color == Color &&
               zone.IsVisible == IsVisible &&
               zone.XMin == XMin &&
               zone.XMax == XMax &&
               zone.YMin == YMin &&
               zone.YMax == YMax &&
               zone.ZMin == ZMin &&
               zone.ZMax == ZMax;


        public override bool Equals(object obj)
        {
            var zone = obj as ControlZoneModel;

            if (zone == this)
                return true;

            if (zone is null ||
                GetHashCode() != zone.GetHashCode())
                return false;

            return
                zone.Name == Name &&
                zone.Color == Color &&
                zone.IsVisible == IsVisible &&
                zone.GeometricZone.Equals(GeometricZone);
        }

        public override int GetHashCode()
        {
            double hashCode = 811243127;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            hashCode = hashCode * -1521134295 + IsVisible.GetHashCode();
            hashCode = hashCode * -1521134295 + GeometricZone.GetHashCode();
            return (int)hashCode;
        }

        public static ControlZone ControlZoneModelToControlZone(ControlZoneModel zoneModel)
            => new ControlZone 
            {
                Name = zoneModel.Name,
                Color = zoneModel.Color,
                IsVisible = zoneModel.IsVisible,
                XMin = zoneModel.XMin,
                XMax = zoneModel.XMax,
                YMin = zoneModel.YMin,
                YMax = zoneModel.YMax,
                ZMin = zoneModel.ZMin,
                ZMax = zoneModel.ZMax,
            };
    }
}
