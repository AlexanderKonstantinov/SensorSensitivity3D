
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

        public Geophone AddGeophone(Geophone geophone)
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
                return null;
            }

            //проверить id
            return geophone;
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

        public Geophone EditGeophone(Geophone geophone)
        {
            using (var context = new Context())
            {
                try
                {
                    var editedGeophone = context.Geophones.First(g => g.Id == geophone.Id);

                    geophone.CopyTo(editedGeophone);

                    context.SaveChanges();                    
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return geophone;
        }
    }
}
