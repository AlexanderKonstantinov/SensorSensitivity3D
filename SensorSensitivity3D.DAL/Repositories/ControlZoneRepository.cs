using System;
using System.Collections.Generic;
using System.Linq;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.DAL.Repositories
{
    public class ControlZoneRepository
    {
        public IEnumerable<ControlZone> GetConfigControlZones(int configId)
        {
            using (var context = new Context())
            {
                return context.ControlZones.Where(g => g.ConfigId == configId).ToList();
            }
        }

        public ControlZone GetControlZone(int id)
        {
            using (var context = new Context())
            {
                return context.ControlZones.FirstOrDefault(z => z.Id == id);
            }
        }

        public bool AddControlZone(ControlZone zone)
        {
            try
            {
                using (var context = new Context())
                {
                    context.Add(zone);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool RemoveControlZone(ControlZone zone)
        {
            try
            {
                using (var context = new Context())
                {
                    context.Remove(zone);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void SaveControlZone(ControlZone zone)
        {
            try
            {
                using (var context = new Context())
                {
                    var editedZone = context.ControlZones.First(g => g.Id == zone.Id);

                    zone.CopyTo(editedZone);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                // Пробуем восстанавить параметры зоны контроля
                try
                {
                    GetControlZone(zone.Id).CopyTo(zone);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void SaveControlZones(IEnumerable<ControlZone> zones)
        {
            try
            {
                using (var context = new Context())
                {
                    foreach (var z in zones)
                        z.CopyTo(context.ControlZones.First(dbZ => dbZ.Id == z.Id));

                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                // Пробуем восстанавить параметры геофонов
                try
                {
                    using (var context = new Context())
                    {
                        foreach (var z in zones)
                            context.ControlZones.First(dbZ => dbZ.Id == z.Id).CopyTo(z);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
