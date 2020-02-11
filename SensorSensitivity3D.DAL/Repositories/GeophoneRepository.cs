
using System;
using System.Collections.Generic;
using System.Linq;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.DAL.Repositories
{
    public class GeophoneRepository
    {
        public IEnumerable<Geophone> GetConfigGeophones(int configId)
        {
            using (var context = new Context())
            {
                return context.Geophones.Where(g => g.ConfigId == configId).ToList();
            }
        }

        public Geophone GetGeophone(int id)
        {
            using (var context = new Context())
            {
                return context.Geophones.FirstOrDefault(g => g.Id == id);
            }
        }

        public bool AddGeophone(Geophone geophone)
        {
            try
            {
                using (var context = new Context())
                {
                    context.Add(geophone);
                    context.SaveChanges();
                }                
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool RemoveGeophone(Geophone geophone)
        {
            try
            {
                using (var context = new Context())
                {
                    context.Remove(geophone);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void SaveGeophone(Geophone geophone)
        {
            try
            {
                using (var context = new Context())
                {
                    var editedGeophone = context.Geophones.First(g => g.Id == geophone.Id);
                    
                    geophone.CopyTo(editedGeophone);
                    context.SaveChanges();
                }                  
            }
            catch (Exception)
            {
                // Пробуем восстанавить параметры геофона
                try
                {
                    GetGeophone(geophone.Id).CopyTo(geophone);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void SaveGeophones(IEnumerable<Geophone> geophones)
        {
            try
            {
                using (var context = new Context())
                {                    
                    foreach (var g in geophones)
                        g.CopyTo(context.Geophones.First(dbG => dbG.Id == g.Id));

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
                        foreach (var g in geophones)
                            context.Geophones.First(dbG => dbG.Id == g.Id).CopyTo(g);
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
