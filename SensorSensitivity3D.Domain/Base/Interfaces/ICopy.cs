
namespace SensorSensitivity3D.Domain.Base.Interfaces
{
    /// <summary>
    /// Поддерживает копирование, при котором состояние экземпляра копируется в целевой объект
    /// </summary>
    /// <typeparam name="TSource">Тип копируемого объекта</typeparam>
    public interface ICopy<TSource>
    {
        void CopyTo(TSource target);
    }
}
