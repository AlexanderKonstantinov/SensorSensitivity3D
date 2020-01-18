
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using devDept.Eyeshot.Entities;
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;

namespace SensorSensitivity3D.Services
{
    public class GeophoneService
    {
        private static readonly GeophoneRepository GeophoneRepository;

        public ObservableCollection<GeophoneModel> GeophoneModels { get; set; }

        static GeophoneService()
        {
            GeophoneRepository = new GeophoneRepository();

            GeophoneModels
        } 

        public IEnumerable<GeophoneModel> GetConfigGeophones(int configId)
            => GeophoneRepository.GetConfigGeophones(configId)
                .Select(g => new GeophoneModel(g));

        public Geophone GetGeophone(int id)
            => GeophoneRepository.GetGeophone(id);

        public bool AddGeophone(GeophoneModel geophone)
            => GeophoneRepository.AddGeophone(geophone.OriginalGeophone);

        public bool RemoveGeophone(GeophoneModel geophone)
            => GeophoneRepository.RemoveGeophone(geophone.OriginalGeophone.Id);


        public IEnumerable<Entity> GetGeophoneEntities()
    }
}
