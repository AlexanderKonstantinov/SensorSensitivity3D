using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Graphics;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.Base;
using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels
{
    public class ZonesViewModel : BaseViewModel
    {
        public event Action<Entity> SelectionEntity;

        public ObservableCollection<ZoneModel> Zones { get; set; }
        public ZoneModel SelectedZone { get; set; }

        public bool ZonesVisibilityAll { get; set; }

        public bool ZonesVisibilityAny
        {
            get => Zones.Any(z => z.Visible);
            set
            {
                foreach (var z in Zones)
                    z.Visible = value;

                UpdateVisibilityParams();
            }
        }



        public ZonesViewModel() { }

        public ZonesViewModel(IList<GeophoneModel> geophones)
        {
            //foreach (var entity in entities)
            //{
            //    entity.Visible = true;
            //}

            List<ZoneModel> geophoneZones = geophones.Select(g => new ZoneModel
            {
                Geophones = new List<Geophone> { g.OriginalGeophone },
                Body = (Solid)g.SensitivitySphere
            }).ToList();

            Zones = new ObservableCollection<ZoneModel>(geophoneZones.Where(z => z.Geophones.Count > 1));

            var rnd = new Random();

            //foreach (var z in Zones)
            //{
            //    z.Color = $"#{Color.FromArgb(70, rnd.Next(256), rnd.Next(256), rnd.Next(256)).Name}";
            //    z.Body.ColorMethod = colorMethodType.byEntity;
            //    z.Body.Color = ColorTranslator.FromHtml(z.Color);
            //    z.Visible = true;
            //}

            //AddEntities(Zones.Select(z => z.Body));



            // Определяем количество доменов по измерениям для задания ёмкости

            int maxSensitivityLimit = int.MinValue;
            double xMin, yMin, zMin, xMax, yMax, zMax;
            xMin = yMin = zMin = double.MaxValue;
            xMax = yMax = zMax = double.MinValue;

            foreach (var g in geophones)
            {
                if (g.R > maxSensitivityLimit)
                    maxSensitivityLimit = g.R;
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

            xMin -= maxSensitivityLimit;
            yMin -= maxSensitivityLimit;
            zMin -= maxSensitivityLimit;

            xMax += maxSensitivityLimit;
            yMax += maxSensitivityLimit;
            zMax += maxSensitivityLimit;
            
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
            while (xDomainNumbers * yDomainNumbers * zDomainNumbers > 10000000) ;

            if (xDomainNumbers * yDomainNumbers * zDomainNumbers > 10000000)
                return;

            // Инициализация доменов
            var domains = new Domain.Models.DomainCub[xDomainNumbers, yDomainNumbers, zDomainNumbers];

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

            var maxSensitivity = double.MinValue;

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

                        if (curDomain.Sensitivity > maxSensitivity)
                            maxSensitivity = curDomain.Sensitivity;

                        domainsEntities.Add(curDomain);
                    }
                }
            }

            // Алгоритм заливки
            Queue<DomainCub> q = new Queue<DomainCub>();

            var groupCount = -1;
            var groups = new Dictionary<int, HashSet<DomainCub>>();

            foreach (var d in domainsEntities)
            {
                if (d.GroupNumber != -1)
                    continue;

                ++groupCount;
                groups.Add(groupCount, new HashSet<DomainCub>( ));

                q.Enqueue(d);

                while (q.Count > 0)
                {
                    var n = q.Peek();

                    if (n.Sensitivity > 0 && n.GroupNumber == -1)
                    {
                        n.GroupNumber = groupCount;
                        groups[groupCount].Add(n);
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
                        groups[groupCount].Add(n);
                    }

                    neighbor = domains[xIndex + gridStep, yIndex, zIndex];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        groups[groupCount].Add(n);
                    }

                    neighbor = domains[xIndex, yIndex - gridStep, zIndex];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        groups[groupCount].Add(n);
                    }

                    neighbor = domains[xIndex, yIndex + gridStep, zIndex];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        groups[groupCount].Add(n);
                    }

                    neighbor = domains[xIndex, yIndex, zIndex - gridStep];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        groups[groupCount].Add(n);
                    }

                    neighbor = domains[xIndex, yIndex, zIndex + gridStep];
                    if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                    {
                        neighbor.GroupNumber = groupCount;
                        q.Enqueue(neighbor);
                        groups[groupCount].Add(n);
                    }
                }
            }


            var levels = new List<double>(10);
            var levelColors = new Dictionary<double, string>(10);
            var subgroups = new Dictionary<IEnumerable<DomainCub>, double>(groups.Count * 10);

            for (var s = maxSensitivity / 10; s <= maxSensitivity; s += maxSensitivity / 10)
            {
                levelColors.Add(s, $"#{Color.FromArgb(0, rnd.Next(256), rnd.Next(256), rnd.Next(256)).Name}");
                levels.Add(s);
            }

            foreach (var g in groups)
                foreach (var level in levels)
                    subgroups.Add(g.Value.Where(d => d.Sensitivity <= level), level);

            _model.Materials.Add(new Material("wood", new Bitmap("Wenge.jpg")));

            foreach (var g in subgroups)
            {
                if (g.Key.Count() < 4)
                    continue;
                
                var groupNumber = g.Key.First().GroupNumber;

                var solid = UtilityEx.ConvexHull(g.Key.Select(d => new Point3D(d.X, d.Y, d.Z)).ToList()).ConvertToSolid();
                
                var z = new ZoneModel
                {
                    Body = solid,
                    Geophones = new List<Geophone> { new Geophone { Name = $"Sensitivity = {g.Value}" } },
                    Visible = true,
                    Color = levelColors[g.Value],
                    GroupNumber = groupNumber
                };
                
                z.Body.ColorMethod = colorMethodType.byEntity;
                z.Body.Color = ColorTranslator.FromHtml(z.Color);
                
                Zones.Add(z);
            }

            // Сортировка объектов по вложенности и создание
            //Zones = new ObservableCollection<ZoneModel>(Zones
            //    .OrderBy(z => z.GroupNumber));

            //for (var i = 0; i < groupCount; ++i)
            //{
            //    var zoneGroup = Zones
            //        .Where(z => z.GroupNumber == i).ToList();

            //    for (var j = 1; j < zoneGroup.Count; ++j)
            //    {
            //        var difference = Solid.Difference((Solid) zoneGroup[j - 1].Body, (Solid) zoneGroup[j].Body);

            //        zoneGroup[j].Body. = difference.FirstOrDefault();
            //    }
            //}

            //foreach (var d in domainsEntities)
            //{
            //    if (d.GroupNumber == -1)
            //        continue;

            //    var mesh = Mesh.CreateSphere(1, 10, 10);
            //    mesh.Translate(d.X, d.Y, d.Z);
            //    mesh.ColorMethod = colorMethodType.byEntity;
            //    mesh.Color = Color.Crimson;

            //    Zones.Add(new ZoneModel
            //    {
            //        Body = mesh,
            //        Visible = true
            //    });
            //}

            AddEntities(Zones.Select(z => z.Body));

            
            
            UpdateVisibility();
            //var curAxeX = GeometricZone.Min.X;
            //do
            //{
            //    var curAxeY = GeometricZone.Min.Y;
            //    do
            //    {
            //        var curAxeZ = GeometricZone.Min.Z;
            //        do
            //        {
            //            domains.Add(new SensitivityDomain
            //            {
            //                Axe1Min = curAxe1,
            //                Axe1Max = curAxe1 + _parameters.DomainSize,
            //                Axe2Min = curAxe2,
            //                Axe2Max = curAxe2 + _parameters.DomainSize,
            //                Axe1Center = curAxe1 + _parameters.DomainSize / 2d,
            //                Axe2Center = curAxe2 + _parameters.DomainSize / 2d,
            //                GeofonesNumber = 0,
            //                Sensitivity = 0
            //            });
            //            curAxeZ += domainSize;
            //        } while (curAxeZ < GeometricZone.Max.Z);
            //        curAxeY += domainSize;
            //    } while (curAxeY < GeometricZone.Max.Y);
            //    curAxeX += domainSize;
            //} while (curAxeX < GeometricZone.Max.X);




            //UpdateVisibilityParams();
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
            SelectedZone = Zones.FirstOrDefault(z => z.Body.Selected);
            return SelectedZone?.ToString();
        }

        protected override void OnDispose()
        {
            Zones?.Clear();
        }
    }
}
