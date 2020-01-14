
using SensorSensitivity3D.DAL.Repositories;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Services
{
    public class DrawingService
    {
        private static readonly DrawingRepository DrawingRepository;

        static DrawingService()
            => DrawingRepository = new DrawingRepository();

        public Drawing GetDrawing(int id)
            => DrawingRepository.GetDrawing(id);

        public bool AddDrawing(Drawing drawing)
            => DrawingRepository.AddDrawing(drawing);
    }
}