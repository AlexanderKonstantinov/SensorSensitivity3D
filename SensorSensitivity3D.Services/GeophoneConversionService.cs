using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using System.Collections.Generic;
using System.Drawing;

namespace SensorSensitivity3D.Services
{
    public static class GeophoneConversionService
    {
        /// <summary>
        /// Создание моделей и сущностей представления геофонов
        /// </summary>
        /// <param name="config">Объект конфигурации, содержащий данные по геофонам</param>
        /// <param name="Geophones">Коллекция геофонов, отображаемая в интерфейсе</param>
        /// <param name="originalGeophones">Словарь, связывающий оригинальные геофоны и их представления</returns>
        /// <param name="geophoneEntities">Словарь с геофонами и сущностями геофонов,
        /// отображаемыми в компоненте</param>
        /// <param name="geophoneSphereEntities">Словарь с геофонами и сущностями сфер чувствительности,
        /// отображаемыми в компоненте</param>
        /// <returns>Все созданные сущности, отображаемые в компоненте</returns>
        public static IEnumerable<Entity> InitGeophoneEntities(
            IList<Geophone> geophones,
            out List<GeophoneModel> Geophones,
            out Dictionary<GeophoneModel, Geophone> _geophoneDic,
            out Dictionary<GeophoneModel, Entity> geophoneEntities,
            out Dictionary<GeophoneModel, Entity> geophoneSphereEntities)
        {
            var count = geophones.Count;

            Geophones = new List<GeophoneModel>(count);
            _geophoneDic = new Dictionary<GeophoneModel, Geophone>(count);
            geophoneEntities = new Dictionary<GeophoneModel, Entity>(count);
            geophoneSphereEntities = new Dictionary<GeophoneModel, Entity>(count);
            var entities = new List<Entity>(count * 2);

            foreach (var g in geophones)
            {
                var geophoneModel = GeophoneToGeophoneModel(g);

                var meshG = CreateGeophoneEntity(g);
                var meshR = CreateGeophoneSphereEntity(g);

                Geophones.Add(geophoneModel);
                _geophoneDic.Add(geophoneModel, g);
                geophoneEntities.Add(geophoneModel, meshG);
                geophoneSphereEntities.Add(geophoneModel, meshR);
                entities.Add(meshG);
                entities.Add(meshR);
            }

            return entities;
        }

        /// <summary>
        /// Создание сферы, обозначающей геофон
        /// </summary>
        /// <param name="geophone">геофон с параметрами</param>
        /// <returns></returns>
        public static Entity CreateGeophoneEntity(Geophone geophone)
        {
            var mesh = Mesh.CreateSphere(3, 20, 20, Mesh.natureType.Smooth);
            mesh.Color = ColorTranslator.FromHtml(geophone.Color);
            mesh.ColorMethod = colorMethodType.byEntity;
            mesh.Translate(geophone.X, geophone.Y, geophone.Z);
            mesh.Visible = geophone.GIsVisible;

            return mesh;
        }

        /// <summary>
        /// Создание сферы, обозначающей зону чувствительности геофона
        /// </summary>
        /// <param name="geophone">геофон с параметрами</param>
        /// <returns></returns>
        public static Entity CreateGeophoneSphereEntity(Geophone geophone)
        {
            var mesh = Mesh.CreateSphere(geophone.R > 0 ? geophone.R : 1, 30, 30, Mesh.natureType.Smooth);
            mesh.Color = ColorTranslator.FromHtml(geophone.Color);
            mesh.ColorMethod = colorMethodType.byEntity;
            mesh.Translate(geophone.X, geophone.Y, geophone.Z);
            mesh.Visible = geophone.SIsVisible;

            return mesh;
        }


        public static Geophone GeophoneModelToGeophone(GeophoneModel model)
            => new Geophone
            {
                HoleNumber = model.HoleNumber,
                Name = model.Name,
                Color = model.Color,
                GIsVisible = model.GIsVisible,
                IsGood = model.IsGood,
                R = model.R,
                SIsVisible = model.SIsVisible,
                X = model.X,
                Y = model.Y,
                Z = model.Z
            };

        public static Geophone CopyConstructor(Geophone g)
            => new Geophone
            {
                HoleNumber = g.HoleNumber,
                Name = g.Name,
                Color = g.Color,
                GIsVisible = g.GIsVisible,
                IsGood = g.IsGood,
                R = g.R,
                SIsVisible = g.SIsVisible,
                X = g.X,
                Y = g.Y,
                Z = g.Z
            };

        /// <summary>
        /// Копировать состояние геофона
        /// </summary>
        /// /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Geophone Copy(Geophone target, Geophone source)
        {
            target.HoleNumber = source.HoleNumber;
            target.Name = source.Name;
            target.Color = source.Color;
            target.GIsVisible = source.GIsVisible;
            target.SIsVisible = source.SIsVisible;
            target.R = source.R;
            target.IsGood = source.IsGood;
            target.X = source.X;
            target.Y = source.Y;
            target.Z = source.Z;

            return target;
        }

        /// <summary>
        /// Копировать состояние геофона в модель геофона
        /// </summary>
        /// /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static GeophoneModel Copy(GeophoneModel target, Geophone source)
        {
            target.OriginalGeophone = source;
            target.HoleNumber = source.HoleNumber;
            target.Name = source.Name;
            target.Color = source.Color;
            target.GIsVisible = source.GIsVisible;
            target.SIsVisible = source.SIsVisible;
            target.R = source.R;
            target.IsGood = source.IsGood;
            target.X = source.X;
            target.Y = source.Y;
            target.Z = source.Z;

            return target;
        }

        public static GeophoneModel GeophoneToGeophoneModel(Geophone g)
            => new GeophoneModel
            {
                OriginalGeophone = g,
                Name = g.Name,
                HoleNumber = g.HoleNumber,
                X = g.X,
                Y = g.Y,
                Z = g.Z,
                IsGood = g.IsGood,
                GIsVisible = g.GIsVisible,
                SIsVisible = g.SIsVisible,
                Color = g.Color,
                R = g.R
            };
    }
}
