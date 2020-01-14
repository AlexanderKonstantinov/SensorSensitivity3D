
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Services
{
    public class ConfigService
    {
        private static readonly ConfigRepository ConfigRepository;

        static ConfigService()
            => ConfigRepository = new ConfigRepository();

        public ObservableCollection<Configuration> GetConfigurations()
            => new ObservableCollection<Configuration>(ConfigRepository.GetConfigurations());

        public Configuration GetConfiguration(int id)
            => ConfigRepository.GetConfiguration(id);
    }
}