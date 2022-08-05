using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;

namespace SensorSensitivity3D.Services
{
    public class GeophoneService
    {
        private static readonly GeophoneRepository GeophoneRepository;
        
        static GeophoneService()
        {
            GeophoneRepository = new GeophoneRepository();
        } 

        public IEnumerable<GeophoneModel> GetConfigGeophones(int configId)
            => GeophoneRepository.GetConfigGeophones(configId)
                .Select(g => new GeophoneModel(g));

        public Geophone GetGeophone(int id)
            => GeophoneRepository.GetGeophone(id);

        public bool AddGeophone(GeophoneModel geophone, int configId)
        {
            var addedGeophone = GeophoneModel.GeophoneModelToGeophone(geophone);
            addedGeophone.ConfigId = configId;

            if (!GeophoneRepository.AddGeophone(addedGeophone))
                return false;

            geophone.OriginalGeophone = addedGeophone;
            return true;
        }

        public bool RemoveGeophone(GeophoneModel geophone)
            => GeophoneRepository.RemoveGeophone(geophone.OriginalGeophone);

        /// <summary>
        /// Сохранить параметры геофона в БД
        /// </summary>
        /// <param name="geophone"></param>
        public void SaveGeophone(GeophoneModel geophone)
        {
            geophone.AcceptChanges();

            GeophoneRepository.SaveGeophone(geophone.OriginalGeophone);
        }

        /// <summary>
        /// Сохраненить параметры всех изменённых геофонов в БД
        /// </summary>
        /// <param name="geophones"></param>
        public void SaveGeophones(IEnumerable<GeophoneModel> geophones)
        {
            var changedGeophones = geophones.Where(g => g.IsChanged).ToList();
            foreach (var g in changedGeophones)
                g.AcceptChanges();

            GeophoneRepository.SaveGeophones(changedGeophones.Select(g => g.OriginalGeophone));
        }

        /// <summary>
        /// Сохранить геофоны в xml-файл
        /// </summary>
        /// <param name="geophones">Список геофонов</param>
        /// <returns></returns>
        public bool SaveToFile(IEnumerable<GeophoneModel> geophones, string filePath)
        {
            try 
            {
                var formatter = new XmlSerializer(geophones.GetType());

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    formatter.Serialize(fs, geophones);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<GeophoneModel> LoadFromFile(string filePath)
        {
            GeophoneModel[] geophones;
            try
            {
                var formatter = new XmlSerializer(typeof(GeophoneModel[]));

                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    geophones = (GeophoneModel[]) formatter.Deserialize(fs);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return geophones;
        }

        public IEnumerable<GeophoneModel> GetGeophonesFromGCSDb(string connectionString, out string errorMessage)
        {
            List<GeophoneModel> geophones = null;
            errorMessage = String.Empty;

            try
            {
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();

                    using (var command = cnn.CreateCommand())
                    {
                        var sensorHoles = new Dictionary<int, int>();

                        command.CommandText = @"
SELECT Sensors.HWID, Holes.Name, Holes.X, Holes.Y, Holes.Z
FROM SensorHole

LEFT OUTER JOIN Sensors
ON SensorHole.SensorID = Sensors.ID

LEFT OUTER JOIN Holes
ON SensorHole.HoleID = Holes.ID

WHERE SensorHole.BeginTime IN (
SELECT MAX(BeginTime) FROM SensorHole Where BeginTime IN (
SELECT MAX(BeginTime) FROM SensorHole GROUP BY HoleID) GROUP BY SensorID)";

                        geophones = new List<GeophoneModel>(sensorHoles.Count);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                geophones.Add(new GeophoneModel
                                {
                                    Name = $"HWID {reader.GetInt32(0)}",
                                    HoleNumber = reader.GetInt32(1),
                                    X = reader.GetDouble(2),
                                    Y = reader.GetDouble(3),
                                    Z = reader.GetDouble(4),
                                    GIsVisible = true,
                                    SIsVisible = true,
                                    IsGood = true
                                });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return geophones;
            }

            return geophones;
        }
    }
    
}
