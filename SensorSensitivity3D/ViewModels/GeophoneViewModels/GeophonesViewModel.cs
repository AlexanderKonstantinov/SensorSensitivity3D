using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SensorSensitivity3D.Domain.Enums;

using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophonesViewModel : BaseViewModel
    {
        private readonly GeophoneService _geophoneService; 

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
        
        public GeophonesViewModel(GeophoneService geophoneService, int configId)
        {
            _geophoneService = geophoneService;

            GeophoneModels = new ObservableCollection<GeophoneModel>
            (
                _geophoneService.GetConfigGeophones(configId).Select(g => new GeophoneModel(g))
            );

            var entities = GeophoneModels
                .Select(g => g.GeophoneEntity)
                .Concat(GeophoneModels.Select(g => g.GeophoneSphereEntity));

            AddEntities(entities);

            

            _model = model;
            _entityList = entityList;
            _entityList.AddRange(entities);

            GeophoneModels = new ObservableCollection<GeophoneModel>(geophoneModels);

            GeophoneViewModel = new GeophoneViewModel(_model, _entityList);
            GeophoneViewModel.Back += SaveNewGeophone;

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
            => _addGeophoneCommand ??= new RelayCommand(ExecuteAddGeophoneCommand);

        public void ExecuteAddGeophoneCommand(object o)
        {
            SelectedGeophone = new GeophoneModel();

            GeophoneModels.Add(SelectedGeophone);

            _entityList.Add(SelectedGeophone.GeophoneEntity);
            _entityList.Add(SelectedGeophone.GeophoneSphereEntity);

            ExecuteSelectGeophoneCommand(SelectedGeophone);

            GeophoneViewModel.ActivateGeophoneViewModel("Добавить геофон", SelectedGeophone);
        }

        private RelayCommand _editGeophoneCommand;
        public ICommand EditGeophoneCommand
            => _editGeophoneCommand ??= new RelayCommand(ExecuteEditGeophoneCommand);

        private void ExecuteEditGeophoneCommand(object obj)
        {
            GeophoneViewModel.ActivateGeophoneViewModel("Редактировать геофон", SelectedGeophone);
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
            {
                geophoneModel.ResetGeophoneSettings();
                _entityList.RemoveRange(new [] {
                    geophoneModel.GeophoneEntity,
                    geophoneModel.GeophoneSphereEntity
                });
            }
            else
            {
                foreach (var g in GeophoneModels)
                    g.ResetGeophoneSettings();

                _entityList.RemoveRange(GeophoneModels.Select(g => g.GeophoneEntity).Concat(GeophoneModels.Select(g => g.GeophoneSphereEntity)));
            }

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
            if (GeophoneViewModel.IsGeophonePanel)
                return;

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
        private void SaveNewGeophone(GeophoneOperation operation)
        {
            ExecuteUnselectGeophoneCommand(SelectedGeophone);


            switch (operation)
            {
                case GeophoneOperation.Save:
                    //TODO обработать
                    if (!_geophoneService.AddGeophone(SelectedGeophone.OriginalGeophone))
                        return;
                    break;
                case GeophoneOperation.SaveAndContinueAdding:
                    ExecuteAddGeophoneCommand(null);
                    break;
                default:
                    GeophoneModels.Remove(SelectedGeophone);
                    _entityList.Remove(SelectedGeophone.GeophoneEntity);
                    _entityList.Remove(SelectedGeophone.GeophoneSphereEntity);
                    break;
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
            GeophoneModels?.Clear();
        }
    }
}
