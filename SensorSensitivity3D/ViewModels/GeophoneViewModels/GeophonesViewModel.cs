using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophonesViewModel : BaseViewModel
    {
        private readonly GeophoneService _geophoneService; 
        private readonly GeophoneConversionService _geophoneConversionService; 
        

        private readonly CustomModel _model;
        private readonly CustomEntityList _entityList;


        public ObservableCollection<GeophoneModel> GeophoneModels { get; set; }

        public const string TRANSPARENT = "Transparent";

        public string GeophonesColor { get; set; }

        private bool _geophonesIsVisible;
        public bool GeophonesIsVisible
        {
            get => _geophonesIsVisible;
            set
            {
                if (_geophonesIsVisible.Equals(value))
                    return;

                foreach (var g in GeophoneModels)
                    g.GIsVisible = value;
                
                _geophonesIsVisible = value;
                OnPropertyChanged(nameof(GeophonesIsVisible));
                _model.Invalidate();
                _model.UpdateVisibleSelection();
            }
        }

        private bool _geophoneSpheresIsVisible;
        public bool GeophoneSpheresIsVisible
        {
            get => _geophoneSpheresIsVisible;
            set
            {
                if (_geophoneSpheresIsVisible.Equals(value))
                    return;

                foreach (var g in GeophoneModels)
                    g.SIsVisible = value;

                _geophoneSpheresIsVisible = value;
                OnPropertyChanged(nameof(GeophoneSpheresIsVisible));

                _model.UpdateVisibleSelection();
                _model.Invalidate();
            }
        }


        public GeophoneModel SelectedGeophone { get; set; }

        public GeophoneViewModel GeophoneViewModel { get; set; }
        

        public GeophonesViewModel() { }

        public GeophonesViewModel(
            CustomModel model,
            CustomEntityList entityList,
            Configuration config)
        {
            _geophoneService = new GeophoneService();
            _geophoneConversionService = new GeophoneConversionService();

            var entities = _geophoneConversionService.InitGeophoneEntities(
                _geophoneService.GetConfigGeophones(config.Id),
                out var geophoneModels);

            _model = model;
            _entityList = entityList;
            _entityList.AddRange(entities);

            GeophoneModels = new ObservableCollection<GeophoneModel>(geophoneModels);

            GeophoneViewModel = new GeophoneViewModel();
            GeophoneViewModel.Back += UpdateGeophoneCollection;

            var firstGeophone = GeophoneModels.FirstOrDefault();
            GeophonesColor =
                firstGeophone != null && GeophoneModels.Skip(1).All(g => g.Color == firstGeophone.Color)
                ? firstGeophone.Color
                : TRANSPARENT;

            UpdateCommonVisibilityParams();
        }

        #region commands

        #region editing of geophones

        private RelayCommand _addGeophoneCommand;
        public ICommand AddGeophoneCommand
            => _addGeophoneCommand ??= new RelayCommand((o)
                =>
            {
                GeophoneViewModel.ActivateGeophoneViewModel(null, new Geophone(), "Добавить геофон");
            });

        private RelayCommand _editGeophoneCommand;
        public ICommand EditGeophoneCommand
            => _editGeophoneCommand ??= new RelayCommand(ExecuteEditGeophoneCommand);

        private void ExecuteEditGeophoneCommand(object obj)
        {
            GeophoneViewModel.ActivateGeophoneViewModel(
                SelectedGeophone.OriginalGeophone,
                _geophoneConversionService.GeophoneModelToGeophone(SelectedGeophone),
                "Редактировать геофон");
        }


        private RelayCommand _removeGeophoneCommand;
        public ICommand RemoveGeophoneCommand
            => _removeGeophoneCommand ??= new RelayCommand(ExecuteRemoveGeophoneCommand);

        private void ExecuteRemoveGeophoneCommand(object o)
        {
            GeophoneModels.Remove(SelectedGeophone);
            _entityList.Remove(SelectedGeophone.GeophoneEntity);
            _entityList.Remove(SelectedGeophone.GeophoneSphereEntity);

            _geophoneService.RemoveGeophone(SelectedGeophone.OriginalGeophone.Id);

            UpdateCommonVisibilityParams();

            _model.Invalidate();
        }

        #endregion

        #region change visibility parameters of geophones

        private RelayCommand _changeColorCommand;
        public ICommand ChangeColorCommand
            => _changeColorCommand ??= new RelayCommand(ExecuteChangeColorCommand);

        private void ExecuteChangeColorCommand(object o)
        {
            var color = o.ToString();

            if (SelectedGeophone is null)
            {
                GeophonesColor = color;
                foreach (var g in GeophoneModels)
                    g.Color = color;
            }
            else
            {
                GeophonesColor = TRANSPARENT;
                SelectedGeophone.Color = color;
            }
            _model.Invalidate();
        }


        private RelayCommand _resetGeophoneCommandCommand;
        public ICommand ResetGeophoneCommand
            => _resetGeophoneCommandCommand ??= new RelayCommand(ExecuteResetGeophoneCommand, CanExecuteResetGeophoneCommandCommand);

        private void ExecuteResetGeophoneCommand(object obj)
        {
            if (obj is GeophoneModel geophoneModel)
                geophoneModel.ResetGeophoneSettings();
            else
                foreach (var g in GeophoneModels)
                    g.ResetGeophoneSettings();

            GeophonesColor = TRANSPARENT;
            _model.Invalidate();
        }

        private bool CanExecuteResetGeophoneCommandCommand(object obj)
            => obj is GeophoneModel geophoneModel
                ? geophoneModel.IsChanged
                : GeophoneModels?.Any(g => g.IsChanged) ?? false;


        private RelayCommand _changeGeophonesVisibilityCommand;
        public ICommand ChangeGeophonesVisibilityCommand
            => _changeGeophonesVisibilityCommand ??= new RelayCommand(ExecuteChangeGeophoneCommand);

        private void ExecuteChangeGeophoneCommand(object obj)
        {
            UpdateCommonVisibilityParams();

            _model.UpdateVisibleSelection();
            _model.Invalidate();
        }

        #endregion

        #region additional commands

        private RelayCommand _selectGeophoneCommand;
        public ICommand SelectGeophoneCommand
            => _selectGeophoneCommand ??= new RelayCommand(ExecuteSelectGeophoneCommand);

        private void ExecuteSelectGeophoneCommand(object obj)
        {
            SelectedGeophone = obj as GeophoneModel;

            if (SelectedGeophone != null)
            {
                SelectedGeophone.GeophoneEntity.Selected = SelectedGeophone.GeophoneSphereEntity.Selected = true;

                _model.UpdateVisibleSelection();
                _model.Invalidate();
            }
        }


        private RelayCommand _unselectGeophoneCommand;
        public ICommand UnselectGeophoneCommand
            => _unselectGeophoneCommand ??= new RelayCommand(ExecuteUnselectGeophoneCommand);

        private void ExecuteUnselectGeophoneCommand(object obj)
        {
            if (SelectedGeophone != null)
            {
                SelectedGeophone.GeophoneEntity.Selected = SelectedGeophone.GeophoneSphereEntity.Selected = false;
                _model.UpdateVisibleSelection();
                _model.Invalidate();
            }

            SelectedGeophone = obj as GeophoneModel;
        }


        private RelayCommand _goToGeophoneCommand;
        public ICommand GoToGeophoneCommand
            => _goToGeophoneCommand ??= new RelayCommand(ExecuteGoToGeophoneCommand, CanExecuteGoToGeophoneCommand);

        private void ExecuteGoToGeophoneCommand(object o)
        {
            _model.GoToEntities(new List<Entity>
            {
                SelectedGeophone.GeophoneEntity,
                SelectedGeophone.GeophoneSphereEntity
            });
        }

        private bool CanExecuteGoToGeophoneCommand(object o)
            => o is GeophoneModel g && (g.GIsVisible || g.SIsVisible);

        #endregion

        #endregion


        public GeophoneModel TrySelectedGeophone()
        {
            var geophone = GeophoneModels.FirstOrDefault(p => p.GeophoneEntity.Selected);

            if (geophone is null)
                geophone = GeophoneModels.FirstOrDefault(p => p.GeophoneSphereEntity.Selected);

            return geophone;
        }

        /// <summary>
        /// Добавление нового геофона в коллекцию или редактирование
        /// имеющегося в коллекции геофона;
        /// обновление изображения
        /// </summary>
        /// <param name="editedGeophone"></param>
        private void UpdateGeophoneCollection(Geophone editedGeophone)
        {
            if (editedGeophone is null)
                return;

            var meshG = _geophoneConversionService.CreateGeophoneEntity(editedGeophone);
            var meshS = _geophoneConversionService.CreateGeophoneSphereEntity(editedGeophone);

            var geophoneModel = GeophoneModels.FirstOrDefault(g => g.OriginalGeophone.Equals(editedGeophone));
            
            // Пользователь добавил новый геофон
            if (geophoneModel is null)
            {
                //TODO обработать
                if (!_geophoneService.AddGeophone(editedGeophone))
                    return;

                geophoneModel = _geophoneConversionService.GeophoneToGeophoneModel(editedGeophone);

                geophoneModel.GeophoneEntity = meshG;
                geophoneModel.GeophoneSphereEntity = meshS;                   

                GeophoneModels.Add(geophoneModel);

                _entityList.Add(meshG);
                _entityList.Add(meshS);
            }

            // Пользователь отредактировал геофон
            else
            {
                geophoneModel = _geophoneConversionService.Copy(geophoneModel, editedGeophone);

                _entityList.Remove(geophoneModel.GeophoneEntity);
                _entityList.Remove(geophoneModel.GeophoneSphereEntity);

                _entityList.Add(meshG);
                _entityList.Add(meshS);
            }

            UpdateCommonVisibilityParams();

            _model.Invalidate();
        }

        private void UpdateCommonVisibilityParams()
        {
            GeophonesIsVisible = GeophoneModels.All(g => g.GIsVisible);
            GeophoneSpheresIsVisible = GeophoneModels.All(g => g.SIsVisible);
        }

        protected override void OnDispose()
        {
            GeophoneModels.Clear();
        }
    }
}
