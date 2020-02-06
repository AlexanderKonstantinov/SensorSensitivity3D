
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

        public bool RenameConfiguration(int configId, string newName)
        {
            try
            {
                var editedConfig = GetConfiguration(configId);

                editedConfig.Name = newName;

                using (var context = new Context())
                {
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

        public bool SaveContext()
        {
            try
            {
                using (var context = new Context())
                {
                    var test = context.Configurations.First();
                    test.Name = "111";
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
