
using System;
using System.Collections.Generic;
using System.Linq;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.DAL.Repositories
{
    public class ConfigRepository
    {
        public IEnumerable<Configuration> GetConfigurations()
        {
            using (var context = new Context())
            {
                return context.Configurations.ToList();
            }
        }

        public Configuration GetConfiguration(int id)
        {
            using (var context = new Context())
            {
                return context.Configurations.FirstOrDefault(c => c.Id == id);
            }
        }

        public bool AddConfiguration(Configuration config)
        {
            try
            {
                using (var context = new Context())
                {
                    context.Configurations.Add(config);
                    context.SaveChanges();
                }
                
                return true;
            }
            catch (Exception)
            {
                //TODO
                return false;
            }
        }

        public bool RemoveConfiguration(Configuration config)
        {
            try
            {
                using (var context = new Context())
                {
                    context.Configurations.Remove(config);
                    context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                //TODO
                return false;
            }
        }

        public void EditConfiguration(Configuration config)
        {
            try
            {
                using (var context = new Context())
                {
                    var editedConfig = context.Configurations.FirstOrDefault(c => c.Id == config.Id);

                    editedConfig.Name = config.Name;
                    editedConfig.SubstratePath = config.SubstratePath;
                    editedConfig.DrawingIsVisible = config.DrawingIsVisible;

                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                //TODO
            }
        }       
    }
}
