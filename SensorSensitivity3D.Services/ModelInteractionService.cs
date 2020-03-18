using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Threading;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using SensorSensitivity3D.Domain.Models;
using Point = System.Windows.Point;
using Region = devDept.Eyeshot.Entities.Region;

namespace SensorSensitivity3D.Services
{
    public static class ModelInteractionService
    {
        

        public static CustomModel _model;
        private static ReadAutodesk _readAutodesk;

        private static HashSet<Entity> _selectedEntities;
        private static HashSet<Entity> _tempEntities;

        public static void Init(CustomModel model)
        {
            _model = model;
            _selectedEntities = new HashSet<Entity>(2);
            _tempEntities = new HashSet<Entity>(2);
        }

        //TODO Нужно тестировать
        public static bool UpdateSubstrate(string substratePath, bool isVisible, ref Drawing drawing)
        {
            if (!File.Exists(substratePath))
                return false;

            Application.Current.Dispatcher?.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));

            _readAutodesk = new ReadAutodesk(substratePath);
            _readAutodesk.DoWork();

            if (drawing?.Entities != null)
                RemoveEntities(drawing.Entities);

            _readAutodesk.AddToScene(_model);

            foreach (var e in _readAutodesk.Entities)
            {
                e.Selectable = false;
                e.Visible = isVisible;
            }

            drawing = new Drawing
            {
                IsVisible = isVisible,
                Name = new FileInfo(substratePath).Name,
                XMin = _model.Entities.BoxMin.X,
                XMax = _model.Entities.BoxMax.X,
                YMin = _model.Entities.BoxMin.Y,
                YMax = _model.Entities.BoxMax.Y,
                ZMin = _model.Entities.BoxMin.Z,
                ZMax = _model.Entities.BoxMax.Z,
                Entities = _readAutodesk.Entities
            };

            return true;
        }

        public static void AddEntities(IEnumerable<Entity> entities)
        {
            _model.Entities.AddRange(entities);

            Invalidate();
        }

        public static void RemoveEntities(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                _model.Entities.Remove(entity);

            Invalidate();
        }

        public static void ReplaceEntities(IEnumerable<Entity> oldEntities, IEnumerable<Entity> newEntities)
        {
            foreach (var e in oldEntities)
                _model.Entities.Remove(e);

            _model.Entities.AddRange(newEntities);
        }


        /// <summary>
        /// Выделяет элементы, снимая выделение с прежних
        /// </summary>
        /// <param name="entities"></param>
        public static void SelectEntity(Entity entity)
        {
            _model.Entities.ClearSelection();

            if (entity != null)
            {
                entity.Selected = true;
                _selectedEntities.Add(entity);
            }

            UpdateVisibility();
        }

        /// <summary>
        /// Выделяет элементы, снимая выделение с прежних
        /// Если entities == null, то просто снимает выделение
        /// </summary>
        /// <param name="entities"></param>
        public static void SelectEntities(IEnumerable<Entity> entities)
        {   
            if (entities == null)
            {
                ClearSelection();
                UpdateVisibility();
                return;
            }

            var isEqual = _selectedEntities.SetEquals(entities);

            if (isEqual)
                return;

            ClearSelection();

            foreach (var e in entities)
            {
                e.Selected = true;
                _selectedEntities.Add(e);

                if (!e.Visible)
                {
                    var temp = (Entity) e.Clone();
                    temp.Visible = true;
                    temp.Selected = true;
                    _tempEntities.Add(temp);
                    _model.Entities.Add(temp);
                }
            }

            UpdateVisibility();
        }

        public static void TestMethod(System.Drawing.Point point)
        {
            _model.GetAllEntitiesUnderMouseCursor(point);
        }

        public static void SwitchEntitiesVisibility(IEnumerable<Entity> entities, bool visibility)
        {
            foreach (var e in entities)
                e.Visible = visibility;

            UpdateVisibility();
        }

        /// <summary>
        /// Масштабирование объектов модели
        /// </summary>
        /// <param name="entities">Объекты, которые требуется масштабировать</param>
        /// <remarks>Если entities == null, то происходит масштабирование выделенных объектов</remarks>
        public static void ZoomFitEntities(IList<Entity> entities = null)
        {
            if (entities == null)
            {
                _model.ZoomFitSelectedLeaves(20);
            }
            else
            {
                // Если объекты невидимые, масштабирование не происходит
                // Поэтому сохраняем значения видимости
                var visibilitiesSafe = entities.Select(e => e.Visible).ToList();

                foreach (var e in entities)
                    e.Visible = true;

                _model.ZoomFit(entities, false, 20);

                // и восстанавливаем первоначальную видимость
                for (int i = 0; i < entities.Count; i++)
                    entities[i].Visible = visibilitiesSafe[i];
            }           

            Invalidate();
        }

        public static void Invalidate()
            => _model.Invalidate();

        public static void UpdateVisibility()
        {
            _model.UpdateVisibleSelection();
            Invalidate();
        }

        public static void ZoomFit()
        {
            _model.ZoomFit();
            Invalidate();
        }


        public static Plane _sectionPlane;
        public static PlanarEntity _sectionEntity;

        private static List<Entity> _zoneEntityBuffer;

        public static void ClickOnViewport(IEnumerable<ZoneModel> zoneGroup)
        {
            // удаляем предыдущее сечение
            _model.Entities.Remove(_sectionEntity);

            if (zoneGroup == null)
            {
                _model.ObjectManipulator.Visible = _model.ObjectManipulator.RotateX.Visible =
                    _model.ObjectManipulator.RotateY.Visible = _model.ObjectManipulator.RotateZ.Visible = false;


                _model.ObjectManipulator.Cancel();
                // восстанавливаем состояние зон

            }
            else
            {
                Entity biggestEntity = null;

                foreach (var zone in zoneGroup)
                {
                    if (biggestEntity is null || biggestEntity.BoxSize.Max < zone.Body.BoxSize.Max)
                        biggestEntity = zone.Body;
                }

                var leftBottomPoint = new Point3D(
                    biggestEntity.BoxMin.X + biggestEntity.BoxSize.X / 2,
                    biggestEntity.BoxMin.Y,
                    biggestEntity.BoxMin.Z);

                _sectionPlane = new Plane(leftBottomPoint, Vector3D.AxisX);

                _sectionEntity = new PlanarEntity(_sectionPlane, (float)biggestEntity.BoxSize.Max);
                
                //_model.Entities.Add(_sectionEntity, Color.Magenta);
                
                Transformation initialTransformation = null;
                bool center = true;
                Point3D rotationPoint = (Point3D) _sectionEntity.EntityData;

                if (_sectionEntity.EntityData is Point3D)
                {
                    center = false;
                    rotationPoint = _sectionEntity.BoxMin;
                }

                if (rotationPoint != null)

                    initialTransformation = new Translation(rotationPoint.X, rotationPoint.Y,
                        rotationPoint.Z);
                else

                    initialTransformation = new Identity();

                _sectionEntity.Selected = true;

                _model.ObjectManipulator.Enable(initialTransformation, center);

                _model.ObjectManipulator.Visible = _model.ObjectManipulator.RotateX.Visible =
                    _model.ObjectManipulator.RotateY.Visible = _model.ObjectManipulator.RotateZ.Visible = true;

                // определяем все объекты, которые на него попадают

                // рассекаем эти объекты
                
                var plate = Solid.CreateBox(
                    0.1,
                    (float)biggestEntity.BoxSize.Max,
                    (float)biggestEntity.BoxSize.Max);

                plate.Translate(leftBottomPoint.X, leftBottomPoint.Y, leftBottomPoint.Z);

                //_model.Entities.Add(plate, Color.Transparent);

                _zoneEntityBuffer = zoneGroup.Select(e => e.Body).ToList();

                var visibleEntities = new List<Entity>(zoneGroup.Count());

                _model.Entities.Add(plate, Color.Transparent);

                foreach (var zone in zoneGroup)
                {
                    var solid = (Solid) zone.Body;

                    var t = Solid.Difference(solid, plate);

                    if (t?.Length > 1)
                    {
                        _model.Entities.Remove(solid);

                        var fstPart = t.ElementAt(0);
                        var sndPart = t.ElementAt(1);

                        var visibleEntity = fstPart.BoxMin.X < sndPart.BoxMin.X
                            ? fstPart
                            : sndPart;

                        visibleEntities.Add(visibleEntity);

                        zone.Body = visibleEntity;

                        _model.Entities.Remove(solid);
                    }
                   

                }

                visibleEntities.Reverse();
                AddEntities(visibleEntities);

            }
           
            Invalidate();
        }

        public static BlockReference CreateBlockReference(ZoneModel zone)
        {
            var blockName = $"{zone.DisplayName} {zone.GroupNumber}";

            ((Mesh)zone.Body).NormalAveragingMode = Mesh.normalAveragingType.Averaged;

            Block bl = new Block(blockName, Point3D.Origin);

            bl.Entities.Add(zone.Body);
            _model.Blocks.Add(bl);

            BlockReference br = new BlockReference(new Identity(), blockName);
            br.ColorMethod = colorMethodType.byEntity;
            br.Color = ColorTranslator.FromHtml(zone.Color);
            
            return br;
        }

        public static void ComputeSection(IEnumerable<Entity> entityGroup)
        {
            //var surfList = new List<Surface>();
            //var sectionLayer = "Section";

            //// explodes the block reference in the individual entities array
            //Entity[] individualEntities = _model.Entities.Explode((BlockReference)entityGroup.First());

            //// finally we fill the surface list
            ////foreach (Entity entity in individualEntities)
            ////{
            ////    if (entity is Surface)
            ////        surfList.Add((Surface)entity);
            ////}

            //_model.Layers.Empty(sectionLayer);

            //// computes the section curves
            //ICurve[] sCurves = Surface.Section(entityGroup, _sectionPlane, 0.01);

            //// add them to on the proper layer
            //foreach (Entity crv in sCurves)
            //    _model.Entities.Add(crv, sectionLayer);
        }


        public static void Focus()
        {
            _model.Focus();
        }

        public static void ClearSelection()
        {
            if (!_selectedEntities.Any())
                return;

            foreach (var e in _selectedEntities)
                e.Selected = false;

            _selectedEntities.Clear();

            foreach (var entity in _tempEntities)
                _model.Entities.Remove(entity);

            _tempEntities.Clear();
        }

        public static void ModelClear()
        {
            _model.Clear();
            UpdateVisibility();
        }
    }
}