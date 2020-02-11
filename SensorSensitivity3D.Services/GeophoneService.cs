
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using devDept.Eyeshot.Entities;
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
    }
}
