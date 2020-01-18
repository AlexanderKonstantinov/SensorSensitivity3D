
using System;
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

        public bool EditConfiguration(Configuration config)
        {
            try
            {
                var old = GetConfiguration(config.Id);

                old.Name = config.Name;
                old.SubstratePath = config.SubstratePath;

                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                //TODO
                return false;
            }
        }
    }
}
