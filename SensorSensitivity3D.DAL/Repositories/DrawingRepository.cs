
using System;
using System.Linq;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.DAL.Repositories
{
    public class DrawingRepository
    {
        private readonly Context _context;

        public DrawingRepository() => _context = Context.Instance;

        public Drawing GetDrawing(int id)
            => _context.Drawings.FirstOrDefault(d => d.Id == id);

        public bool AddDrawing(Drawing drawing)
        {
            try
            {
                _context.Drawings.Add(drawing);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
