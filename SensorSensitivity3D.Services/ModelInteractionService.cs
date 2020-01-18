using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        private static CustomEntityList _entityList;
        private static ReadAutodesk _readAutodesk;

        private static Entity _selectedEntity;
        //private static IEnumerable<Entity> _selectableEntities;
        

        public static void Init(CustomModel model) => _model = model;

        public static void SelectEntity(Entity entity)
        {
            if (_selectedEntity != null)
                _selectedEntity.Selected = false;

            if (entity != null)
            {
                entity.Selected = true;
                _selectedEntity = entity;
            }

            _model.Invalidate();
        }

        //TODO Нужно тестировать
        public static bool UpdateSubstrate(string substratePath)
        {
            if (!File.Exists(substratePath) || substratePath == _readAutodesk?.Path)
                return false;

            Application.Current.Dispatcher?.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));
            
            _readAutodesk = new ReadAutodesk(substratePath);
            _readAutodesk.DoWork();

            //_model.CustomEntityList.RemoveRange(_readAutodesk.Entities);

            _readAutodesk.AddToScene(_model);

            foreach (var e in _readAutodesk.Entities)
                e.Selectable = false;

            return true;
        }

        public static void AddEntities(IEnumerable<Entity> entities)
        {
            _model.CustomEntityList.AddRange(entities);
        }
    }
}