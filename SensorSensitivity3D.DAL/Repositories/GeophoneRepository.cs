
using System;
using System.Collections.Generic;
using System.Linq;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.DAL.Repositories
{
    public class GeophoneRepository
    {
        private readonly Context _context;

        public GeophoneRepository() => _context = Context.Instance;

        public IEnumerable<Geophone> GetConfigGeophones(int configId)
            => _context.Geophones.Where(g => g.ConfigId == configId);

        public Geophone GetGeophone(int id)
            => _context.Geophones.FirstOrDefault(g => g.Id == id);

        public bool AddGeophone(Geophone geophone)
        {
            try
            {
                // Временно
                var maxId = _context.Geophones.Select(g => g.Id).Max();
                geophone.Id = maxId + 1;

                _context.Geophones.Add(geophone);
                //_context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveGeophone(int id)
        {
            var geophoneIndex = _context.Geophones.FindIndex(g => g.Id == id);

            try
            {
                _context.Geophones.RemoveAt(geophoneIndex);
                //_context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
