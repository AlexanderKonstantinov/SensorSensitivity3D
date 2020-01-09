namespace SensorSensitivity3D.Domain.Base.Interfaces
{
    public interface INamedEntity : IBaseEntity
    {
        string Name { get; set; }
    }
}
