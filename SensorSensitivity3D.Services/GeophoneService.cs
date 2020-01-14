
using System.Collections.ObjectModel;
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Services
{
    public class GeophoneService
    {
        private static readonly GeophoneRepository GeophoneRepository;

        static GeophoneService() 
            => GeophoneRepository = new GeophoneRepository();

        public ObservableCollection<Geophone> GetConfigGeophones(int configId)
            => new ObservableCollection<Geophone>(GeophoneRepository.GetConfigGeophones(configId));

        public Geophone GetGeophone(int id)
            => GeophoneRepository.GetGeophone(id);

        public bool AddGeophone(Geophone geophone)
            => GeophoneRepository.AddGeophone(geophone);

        public bool RemoveGeophone(int id)
            => GeophoneRepository.RemoveGeophone(id);
    }
}
