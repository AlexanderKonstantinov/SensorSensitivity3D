
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

        /// <summary>
        /// Добавление новой конфигурации в БД
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddConfiguration(Configuration config)
            => ConfigRepository.AddConfiguration(config);

        /// <summary>
        /// Сохранение конфигурации в базе данных
        /// </summary>
        /// <returns>Успешность сохранения</returns>
        public bool SaveContext()
            => ConfigRepository.SaveContext();
    }
}