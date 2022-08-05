using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using devDept.Geometry;
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Models;

namespace SensorSensitivity3D.Services
{
    public class ControlZoneService
    {
        private static readonly ControlZoneRepository ControlZoneRepository;

        private readonly GeophoneService _geophoneService;

        static ControlZoneService()
        {
            ControlZoneRepository = new ControlZoneRepository();
        }

        public ControlZoneService(GeophoneService gephoneService)
        {
            _geophoneService = gephoneService;
        }

        public IEnumerable<ControlZoneModel> GetConfigControlZones(int configId)
            => ControlZoneRepository.GetConfigControlZones(configId)
                .Select(z => new ControlZoneModel(z));

        public bool AddControlZone(ControlZoneModel zone, int configId)
        {
            var addedZone = ControlZoneModel.ControlZoneModelToControlZone(zone);
            addedZone.ConfigId = configId;

            if (!ControlZoneRepository.AddControlZone(addedZone))
                return false;

            zone.SavedZone = addedZone;
            return true;
        }

        public bool RemoveControlZone(ControlZoneModel zone)
            => ControlZoneRepository.RemoveControlZone(zone.SavedZone);

        /// <summary>
        /// Сохранить параметры зоны контроля в БД
        /// </summary>
        /// <param name="controlZone"></param>
        public void SaveControlZone(ControlZoneModel controlZone)
        {
            controlZone.AcceptChanges();

            ControlZoneRepository.SaveControlZone(controlZone.SavedZone);
        }

        /// <summary>
        /// Сохранить параметры всех изменённых зон контроля в БД
        /// </summary>
        /// <param name="zones"></param>
        public void SaveControlZones(IEnumerable<ControlZoneModel> zones)
        {
            var changedZones = zones.Where(g => g.IsChanged).ToList();
            foreach (var z in changedZones)
                z.AcceptChanges();

            ControlZoneRepository.SaveControlZones(changedZones.Select(z => z.SavedZone));
        }

        public IEnumerable<ControlZoneModel> GetZonesFromGCSDb(string connectionString, out string errorMessage)
        {
            List<ControlZoneModel> zones = null;
            errorMessage = String.Empty;

            try
            {
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();

                    using (var command = cnn.CreateCommand())
                    {
                        command.CommandText = 
@"
SELECT zName, XMIN, YMIN, ZMIN, XMAX, YMAX, ZMAX
FROM Zones
";
                        zones = new List<ControlZoneModel>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                zones.Add(new ControlZoneModel
                                {
                                    Name = reader.GetString(0).Replace(" ", ""),
                                    XMin = (int)reader.GetDouble(1),
                                    XMax = (int)reader.GetDouble(2),
                                    YMin = (int)reader.GetDouble(3),
                                    YMax = (int)reader.GetDouble(4),
                                    ZMin = (int)reader.GetDouble(5),
                                    ZMax = (int)reader.GetDouble(6),
                                    IsVisible = true,
                                    IsCalculated = false
                                });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return zones;
            }

            return zones;
        }

        /// <summary>
        /// Расчёт чувствительности в пространственной зоне
        /// Перед расчетом сохраняются параметры зон и геофонов, влияющих расчет
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="geophones">all geophones</param>
        public IEnumerable<SensitivityZone> CalculationSensitivityZones(GeometricZone zone, IEnumerable<GeophoneModel> geophones)
        {
            SelectGeophonesInZone(ref zone, ref geophones);

            if (geophones.Count() == 0)
                return null;

            var domainList = CalculationDomains(zone, geophones, out SensitivityDomain [,,] allDomains, out int domainSize);

            var zones = SeparateSensitivityDomains(zone, domainList, allDomains, domainSize);

            return zones;
        }

        /// <summary>
        /// Find all geophones in geometric zone
        /// </summary>
        private void SelectGeophonesInZone(ref GeometricZone zone, ref IEnumerable<GeophoneModel> geophones)
        {
            var geophoneList = new List<GeophoneModel>();

            foreach (var g in geophones.Where(g => g.IsGood))
            {
                if (g.X + g.R < zone.XMin || g.X - g.R > zone.XMax ||
                    g.Y + g.R < zone.YMin || g.Y - g.R > zone.YMax ||
                    g.Z + g.R < zone.ZMin || g.Z - g.R > zone.ZMax)
                    continue;

                _geophoneService.SaveGeophone(g);
                geophoneList.Add(g);
            }

            int xMin, yMin, zMin, xMax, yMax, zMax;
            xMin = yMin = zMin = int.MaxValue;
            xMax = yMax = zMax = int.MinValue;

            foreach (var g in geophoneList)
            {
                if (g.X - g.R < xMin)
                    xMin = (int)g.X - g.R;
                if (g.Y - g.R < yMin)
                    yMin = (int)g.Y - g.R;
                if (g.Z - g.R < zMin)
                    zMin = (int)g.Z - g.R;
                if (g.X + g.R > xMax)
                    xMax = (int)g.X + g.R;
                if (g.Y + g.R > yMax)
                    yMax = (int)g.Y + g.R;
                if (g.Z + g.R > zMax)
                    zMax = (int)g.Z + g.R;
            }

            var newZone = new GeometricZone
            {
                XMin = xMin > zone.XMin ? xMin : zone.XMin,
                XMax = xMax < zone.XMax ? xMax : zone.XMax,
                YMin = yMin > zone.YMin ? yMin : zone.YMin,
                YMax = yMax < zone.YMax ? yMax : zone.YMax,
                ZMin = zMin > zone.ZMin ? zMin : zone.ZMin,
                ZMax = zMax < zone.ZMax ? zMax : zone.ZMax
            };

            geophones = geophoneList;
            zone = newZone;
        }

    
        private IEnumerable<SensitivityDomain> CalculationDomains(GeometricZone zone, IEnumerable<GeophoneModel> geophones, out SensitivityDomain [,,] allDomains, out int domainSize)
        {
            // Размер домена
            domainSize = 0;
            
            int xDomainNumbers, yDomainNumbers, zDomainNumbers;

            // Определяем количество доменов по измерениям для задания ёмкости (с защитой от переполнения)
            do
            {
                ++domainSize;
                xDomainNumbers = (zone.XMax - zone.XMin) / domainSize;
                yDomainNumbers = (zone.YMax - zone.YMin) / domainSize;
                zDomainNumbers = (zone.ZMax - zone.ZMin) / domainSize;
            }
            while (xDomainNumbers * yDomainNumbers * zDomainNumbers > 10000000);

            // Инициализация доменов
            allDomains = new SensitivityDomain[xDomainNumbers, yDomainNumbers, zDomainNumbers];

            var curX = zone.XMin;
            for (var x = 0; x < xDomainNumbers; ++x)
            {
                var curY = zone.YMin;
                for (var y = 0; y < yDomainNumbers; ++y)
                {
                    var curZ = zone.ZMin;
                    for (var z = 0; z < zDomainNumbers; ++z)
                    {
                        allDomains[x, y, z] = new SensitivityDomain
                        {
                            X = curX,
                            Y = curY,
                            Z = curZ,
                            GeophoneCount = 0,
                            Sensitivity = 0,
                            GroupNumber = -1
                        };
                        curZ += domainSize;
                    }
                    curY += domainSize;
                }
                curX += domainSize;
            }

            // Расчет чувствительности и
            // выбор доменов с ненулевой чувствительностью
            var domainList = new List<SensitivityDomain>(xDomainNumbers * yDomainNumbers * zDomainNumbers);
            for (var x = 0; x < xDomainNumbers; ++x)
            {
                for (var y = 0; y < yDomainNumbers; ++y)
                {
                    for (var z = 0; z < zDomainNumbers; ++z)
                    {
                        var curDomain = allDomains[x, y, z];
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

                        if (curDomain.GeophoneCount < 4)
                        {
                            curDomain.Sensitivity = 0;
                            continue;
                        }

                        curDomain.Sensitivity *= curDomain.GeophoneCount;

                        domainList.Add(curDomain);
                    }
                }
            }

            return domainList;
        }

        
        /// <summary>
        /// Разделение доменов на отдельные пространственные группы с помощью алгоритма заливки
        /// </summary>
        public List<SensitivityZone> SeparateSensitivityDomains(GeometricZone zone, IEnumerable<SensitivityDomain> domainList, SensitivityDomain [,,] allDomains, int domainSize)
        {
            // Шаг между точками
            var gridStep = 1;

            var xLength = allDomains.GetLength(0);
            var yLength = allDomains.GetLength(1);
            var zLength = allDomains.GetLength(2);

            var groupCount = -1;

            var domainGroups = new List<SensitivityZone>();

            var q = new Queue<SensitivityDomain>();

            foreach (var d in domainList)
            {
                if (d.GroupNumber != -1)
                    continue;

                ++groupCount;

                var group = new SensitivityZone();
                group.GroupNumber = groupCount;
                group.Domains = new HashSet<SensitivityDomain>();
                domainGroups.Add(group);

                q.Enqueue(d);

                while (q.Count > 0)
                {
                    var n = q.Peek();

                    if (n.Sensitivity > 0 && n.GroupNumber == -1)
                    {
                        n.GroupNumber = groupCount;
                        group.Domains.Add(n);
                    }

                    q.Dequeue();

                    var xIndex = (n.X - zone.XMin) / domainSize;
                    var yIndex = (n.Y - zone.YMin) / domainSize;
                    var zIndex = (n.Z - zone.ZMin) / domainSize;

                    SensitivityDomain neighbor;
                    if (xIndex - gridStep >= 0)
                    {
                        neighbor = allDomains[xIndex - gridStep, yIndex, zIndex];
                        if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                        {
                            neighbor.GroupNumber = groupCount;
                            q.Enqueue(neighbor);
                            group.Domains.Add(n);
                        }
                    }

                    if (xIndex + gridStep < xLength)
                    {
                        neighbor = allDomains[xIndex + gridStep, yIndex, zIndex];
                        if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                        {
                            neighbor.GroupNumber = groupCount;
                            q.Enqueue(neighbor);
                            group.Domains.Add(n);
                        }
                    }

                    if (yIndex - gridStep >= 0)
                    {
                        neighbor = allDomains[xIndex, yIndex - gridStep, zIndex];
                        if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                        {
                            neighbor.GroupNumber = groupCount;
                            q.Enqueue(neighbor);
                            group.Domains.Add(n);
                        }
                    }

                    if (yIndex + gridStep < yLength)
                    {
                        neighbor = allDomains[xIndex, yIndex + gridStep, zIndex];
                        if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                        {
                            neighbor.GroupNumber = groupCount;
                            q.Enqueue(neighbor);
                            group.Domains.Add(n);
                        }
                    }

                    if (zIndex - gridStep >= 0)
                    {
                        neighbor = allDomains[xIndex, yIndex, zIndex - gridStep];
                        if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                        {
                            neighbor.GroupNumber = groupCount;
                            q.Enqueue(neighbor);
                            group.Domains.Add(n);
                        }
                    }
                   

                    if (zIndex + gridStep < zLength)
                    {
                        neighbor = allDomains[xIndex, yIndex, zIndex + gridStep];
                        if (neighbor.Sensitivity > 0 && neighbor.GroupNumber == -1)
                        {
                            neighbor.GroupNumber = groupCount;
                            q.Enqueue(neighbor);
                            group.Domains.Add(n);
                        }
                    }
                }
            }

            return domainGroups;
        }
    }
}
