
using System.Collections.Generic;
using System.Linq;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.DAL.Repositories
{
    public class ConfigRepository
    {
        private readonly Context _context;

        public ConfigRepository() => _context = Context.Instance;

        public IEnumerable<Configuration> GetConfigurations()
            => _context.Configurations;

        public Configuration GetConfiguration(int id) 
            => _context.Configurations.FirstOrDefault(c => c.Id == id);
    }
}
