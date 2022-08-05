using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.Base;
using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels
{
    public class ZonesViewModel : BaseViewModel
    {
        private double _currentSensitivityLevel;

        public double CurrentSensitivityLevel
        {
            get => _currentSensitivityLevel;
            set
            {
                _currentSensitivityLevel = value;
                UpdateZones();
                UpdateVisibility();
                OnPropertyChanged(nameof(CurrentSensitivityLevel));
            }
        }
        public double MaxSensitivityLevel { get; set; }

        public event Action<Entity> SelectionEntity;

        public ObservableCollection<ZoneModel> Zones { get; set; }
        public ZoneModel SelectedZone { get; set; }

        public bool ZonesVisibilityAll { get; set; }

        public bool ZonesVisibilityAny
        {
            get => Zones?.Any(z => z.Visible) ?? false;
            set
            {
                if (Zones == null || !Zones.Any())
                    return;

                foreach (var z in Zones)
                    z.Visible = value;

                UpdateVisibilityParams();
            }
        }


        public ZonesViewModel() { }

        public void CalcZones(IList<GeophoneModel> geophones)
        {
            if (!geophones.Any())
                return;

            // Определяем количество доменов по измерениям для задания ёмкости

            int maxSensitivityRadius = int.MinValue;
            double xMin, yMin, zMin, xMax, yMax, zMax;
            xMin = yMin = zMin = double.MaxValue;
            xMax = yMax = zMax = double.MinValue;

            foreach (var g in geophones)
            {
                if (g.R > maxSensitivityRadius)
                    maxSensitivityRadius = g.R;
                if (g.X < xMin)
                    xMin = g.X;
                if (g.Y < yMin)
                    yMin = g.Y;
                if (g.Z < zMin)
                    zMin = g.Z;
                if (g.X > xMax)
                    xMax = g.X;
                if (g.Y > yMax)
                    yMax = g.Y;
                if (g.Z > zMax)
                    zMax = g.Z;
            }

            xMin -= maxSensitivityRadius;
            yMin -= maxSensitivityRadius;
            zMin -= maxSensitivityRadius;

            xMax += maxSensitivityRadius;
            yMax += maxSensitivityRadius;
            zMax += maxSensitivityRadius;

            var domainSize = 1;
            int xDomainNumbers, yDomainNumbers, zDomainNumbers;

            // защита от переполнения
            do
            {
                ++domainSize;
                xDomainNumbers = (int)((xMax - xMin) / domainSize + 1);
                yDomainNumbers = (int)((yMax - yMin) / domainSize + 1);
                zDomainNumbers = (int)((zMax - zMin) / domainSize + 1);
            }
            while (xDomainNumbers * yDomainNumbers * zDomainNumbers > 10000000);

            if (xDomainNumbers * yDomainNumbers * zDomainNumbers > 10000000)
                return;

            // Инициализация доменов
            var domains = new DomainCub[xDomainNumbers, yDomainNumbers, zDomainNumbers];

            var curAxeX = xMin;
            for (var x = 0; x < xDomainNumbers; ++x)
            {
                var curAxeY = yMin;
                for (var y = 0; y < yDomainNumbers; ++y)
                {
                    var curAxeZ = zMin;
                    for (var z = 0; z < zDomainNumbers; ++z)
                    {
                        domains[x, y, z] = new DomainCub
                        {
                            X = curAxeX,
                            Y = curAxeY,
                            Z = curAxeZ,
                            GroupNumber = -1

                        };
                        curAxeZ += domainSize;
                    }
                    curAxeY += domainSize;
                }
                curAxeX += domainSize;
            }

            // расчет чувствительности и
            // выбор доменов с ненулевой чувствительностью

            var gridStep = 1;

            MaxSensitivityLevel = double.MinValue;

            var domainsEntities = new List<DomainCub>(xDomainNumbers * yDomainNumbers * zDomainNumbers);
            for (var x = 0; x < xDomainNumbers; x += gridStep)
            {
                for (var y = 0; y < yDomainNumbers; y += gridStep)
                {
                    for (var z = 0; z < zDomainNumbers; z += gridStep)
                    {
                        var curDomain = domains[x, y, z];
                        foreach (var g in geophones)
                        {
                            var distance = Math.Sqrt(
                                Math.Pow(g.X - curDomain.X, 2) +
                                Math.Pow(g.Y - curDomain.Y, 2) +
                                Math.Pow(g.Z - curDomain.Z, 2));

                            if (!(distance <= g.R)) continue;

                            ++curDomain.GeophoneCount;
                            curDomain.Sensitivity += 1 - Math.Sqrt(distance / g.R);
                        }

                        if (curDomain.GeophoneCount < 2)
                        {
                            curDomain.Sensitivity = 0;
                            continue;
                        }

                        curDomain.Sensitivity *= curDomain.GeophoneCount;

                        if (curDomain.Sensitivity > MaxSensitivityLevel)
                            MaxSensitivityLevel = curDomain.Sensitivity;

                        domainsEntities.Add(curDomain);
                    }
                }
            }

            // Алгоритм заливки, инициализация зон
            var groupCount = -1;
            Zones = new ObservableCollection<ZoneModel>();

            var rnd = new Random();

            Queue<DomainCub> q = new Queue<DomainCub>();

            foreach (var d in domainsEntities)
            {
                if (d.GroupNumber != -1)
                    continue;

                ++groupCount;

                var zone = new ZoneModel();
                zone.Visible = true;
                zone.GroupNumber = groupCount;
                zone.Domains = new HashSet<DomainCub>();
                Zones.Add(zone);

                q.Enqueue(d);

                while (q.Count > 0)
                {
                    var n = q.Peek();

                    if (n.Sensitivity > 0 && n.GroupNumber == -1)
                    {
                        n.GroupNumber = groupCount;
                        zone.Domains.Add(n);
                    }

                    q.Dequeue();

                    var xIndex = (int)((n.X - xMin) / domainSize);
                    var yIndex = (int)((n.Y - yMin) / domainSize);
                    var zIndex = (int)((n.Z - zMin) / domainSize);

                    var neighbor = domains[xIndex - gridStep, yIndex, zIndex];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        zone.Domains.Add(n);
                    }

                    neighbor = domains[xIndex + gridStep, yIndex, zIndex];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        zone.Domains.Add(n);
                    }

                    neighbor = domains[xIndex, yIndex - gridStep, zIndex];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        zone.Domains.Add(n);
                    }

                    neighbor = domains[xIndex, yIndex + gridStep, zIndex];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        zone.Domains.Add(n);
                    }

                    neighbor = domains[xIndex, yIndex, zIndex - gridStep];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        zone.Domains.Add(n);
                    }

                    neighbor = domains[xIndex, yIndex, zIndex + gridStep];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        zone.Domains.Add(n);
                    }
                }
            }

            CurrentSensitivityLevel = MaxSensitivityLevel;

            UpdateVisibilityParams();
        }
        

        private RelayCommand _changeZonesVisibility;
        public ICommand ChangeZonesVisibilityCommand
            => _changeZonesVisibility ??= new RelayCommand(ExecuteChangeZonesVisibilityCommand);

        private void ExecuteChangeZonesVisibilityCommand(object o)
        {
            if (SelectedZone is null)
            {
                foreach (var g in Zones)
                    g.Visible = ZonesVisibilityAll;
            }

            UpdateZones();
            UpdateVisibilityParams();
        }


        private RelayCommand _selectZoneCommand;
        public ICommand SelectZoneCommand
            => _selectZoneCommand ??= new RelayCommand(ExecuteSelectZoneCommand);

        private void ExecuteSelectZoneCommand(object obj)
        {
            SelectedZone = obj as ZoneModel;

            SelectionEntity?.Invoke(SelectedZone?.Body);
        }

        private RelayCommand _goToZoneCommand;
        public ICommand GoToZoneCommand
            => _goToZoneCommand ??= new RelayCommand(ExecuteGoToZoneCommand);

        private void ExecuteGoToZoneCommand(object o)
        {
            ZoomFitEntities(new [] { SelectedZone.Body });
        }


        private void UpdateZones()
        {
            RemoveEntities(Zones.Select(z => z.Body));
            
            foreach (var zone in Zones)
            {
                zone.Body = null;

                var domains = zone.Domains
                    .Where(d => d.Sensitivity <= CurrentSensitivityLevel);

                if (!domains.Any())
                    continue;

                zone.Body = UtilityEx.ConvexHull(domains
                    .Select(d => new Point3D(d.X, d.Y, d.Z))
                    .ToList());
                zone.Body.ColorMethod = colorMethodType.byEntity;
                zone.Body.Color = Color.IndianRed;
            }
            
            AddEntities(Zones
                .Where(z => z.Body != null && z.Body.IsValid())
                .Select(z => z.Body));
        }

        private void UpdateVisibilityParams()
        {
            if (!Zones.Any())
                return;

            ZonesVisibilityAll = Zones.All(z => z.Visible);

            UpdateVisibility();

            OnPropertyChanged(nameof(ZonesVisibilityAny));
        }

        public string TrySelectZone()
        {
            SelectedZone = Zones?.FirstOrDefault(z => z.Body.Selected);
            return SelectedZone?.ToString();
        }

        protected override void OnDispose()
        {
            Zones?.Clear();
        }
    }
}
