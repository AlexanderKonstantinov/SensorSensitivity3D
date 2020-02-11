
using System.Collections.ObjectModel;
using System.IO;
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Services
{
    public class ConfigService
    {
        private static readonly ConfigRepository ConfigRepository;

        static ConfigService()
        {
            ConfigRepository = new ConfigRepository();
        } 

        public ObservableCollection<Configuration> GetConfigurations()
            => new ObservableCollection<Configuration>(ConfigRepository.GetConfigurations());
        
        public Configuration GetConfiguration(int id)
            => ConfigRepository.GetConfiguration(id);

        /// <summary>
        /// Добавление новой конфигурации в БД
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddConfiguration(Configuration config)
            => ConfigRepository.AddConfiguration(config);

        public bool RemoveConfiguration(Configuration config)
            => ConfigRepository.RemoveConfiguration(config);

        /// <summary>
        /// Редактирование конфигурации (имени, пути к подложке и видимости подложки)
        /// </summary>
        /// <param name="config"></param>
        public bool EditConfiguration(Configuration config)
            => ConfigRepository.EditConfiguration(config);
    }
}