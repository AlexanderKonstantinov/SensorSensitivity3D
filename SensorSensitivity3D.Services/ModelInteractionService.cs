using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using SensorSensitivity3D.Domain.Models;

namespace SensorSensitivity3D.Services
{
    public static class ModelInteractionService
    {
        private static CustomModel _model;
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
        /// Если entities == null, то просто снимает выделениеЫ
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