
using System.Collections.Generic;
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Services
{
    public class GeophoneService
    {
        private static readonly GeophoneRepository GeophoneRepository;

        static GeophoneService() 
            => GeophoneRepository = new GeophoneRepository();

        public IEnumerable<Geophone> GetConfigGeophones(int configId)
            => GeophoneRepository.GetConfigGeophones(configId);

        public Geophone GetGeophone(int id)
            => GeophoneRepository.GetGeophone(id);

        public bool AddGeophone(Geophone geophone)
            => GeophoneRepository.AddGeophone(geophone);

        public bool RemoveGeophone(int id)
            => GeophoneRepository.RemoveGeophone(id);
    }
}
