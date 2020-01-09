using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophonesViewModel : BaseViewModel
    {
        public const string TRANSPARENT = "Transparent";

        public readonly CustomModel Model;
        public readonly CustomEntityList EntityList;

        public bool GeophonesIsVisible { get; set; }
        public bool GeophoneSpheresIsVisible { get; set; }
        public string GeophonesColor { get; set; }
        
        public ObservableCollection<GeophoneModel> GeophoneModels { get; set; }

        private Dictionary<GeophoneModel, Geophone> _geophones;

        private Dictionary<GeophoneModel, Entity> _geophoneEntities;
        private Dictionary<GeophoneModel, Entity> _geophoneSphereEntities;

        public GeophoneViewModel GeophoneViewModel { get; set; }
        public GeophoneModel SelectedGeophone { get; set; }
        public string SelectedColor { get; set; }


        public GeophonesViewModel() { }
        public GeophonesViewModel(
            CustomModel model,
            CustomEntityList entityList,
            IList<Geophone> geophones)
        {
            var entities = GeophoneConversionService.InitGeophoneEntities(
                    geophones,
                    out List<GeophoneModel> geophoneModels,
                    out _geophones,
                    out _geophoneEntities,
                    out _geophoneSphereEntities);


            Model = model;
            EntityList = entityList;
            EntityList.AddRange(entities);

            GeophoneModels = new ObservableCollection<GeophoneModel>(geophoneModels);

            var firstGeophone = GeophoneModels.FirstOrDefault();
            GeophonesColor =
                firstGeophone != null && GeophoneModels.Skip(1).All(g => g.Color == firstGeophone.Color)
                ? firstGeophone.Color
                : TRANSPARENT;

            GeophonesIsVisible = GeophoneModels.All(g => g.GIsVisible);
            GeophoneSpheresIsVisible = GeophoneModels.All(g => g.SIsVisible);            
        }

        #region change geophone list commands

        private RelayCommand _addGeophoneCommand;
        public ICommand AddGeophoneCommand
            => _addGeophoneCommand
               ?? (_addGeophoneCommand = new RelayCommand(ExecuteAddGeophoneCommand));

        private void ExecuteAddGeophoneCommand(object obj)
        {
            GeophoneViewModel = new GeophoneViewModel(null, "Добавить геофон");

            GeophoneViewModel.Back += g => UpdateGeophoneCollection(g);
        }


        #endregion

        #region geophone items commands

        #region reset geophone settings
        private RelayCommand _resetGeophoneCommandCommand;
        public ICommand ResetGeophoneCommand
            => _resetGeophoneCommandCommand
               ?? (_resetGeophoneCommandCommand = new RelayCommand(ExecuteResetGeophoneCommand, CanExecuteResetGeophoneCommandCommand));

        private void ExecuteResetGeophoneCommand(object obj)
        {
            if (obj is GeophoneModel geophoneModel)
            {
                geophoneModel.ResetGeophoneSettings();
                _geophoneEntities[geophoneModel].Visible = geophoneModel.GIsVisible;
                _geophoneSphereEntities[geophoneModel].Visible = geophoneModel.SIsVisible;
                _geophoneEntities[geophoneModel].Color =
                    _geophoneSphereEntities[geophoneModel].Color =
                    ColorTranslator.FromHtml(geophoneModel.Color);
            }
            else
            {
                foreach (var g in GeophoneModels)
                {
                    g.ResetGeophoneSettings();
                    _geophoneEntities[g].Visible = g.GIsVisible;
                    _geophoneSphereEntities[g].Visible = g.SIsVisible;
                    _geophoneEntities[g].Color =
                        _geophoneSphereEntities[g].Color =
                        ColorTranslator.FromHtml(g.Color);
                }
            }
            GeophonesColor = TRANSPARENT;
            Model.Invalidate();
        }

        private bool CanExecuteResetGeophoneCommandCommand(object obj)
            => obj is GeophoneModel geophoneModel
            ? geophoneModel.IsChanged
            : GeophoneModels?.Any(g => g.IsChanged) ?? false;
        #endregion

        #region change geophone visibility settings

        private RelayCommand _geophoneVisibilityCommand;
        /// <summary>
        /// Включение/отключение геофонов
        /// Если null, то это для всех геофонов
        /// </summary>
        public ICommand GeophoneVisibilityCommand
            => _geophoneVisibilityCommand
            ?? (_geophoneVisibilityCommand = new RelayCommand(ExecuteGeophoneVisibilityCommand));

        private void ExecuteGeophoneVisibilityCommand(object o)
        {
            if (o is GeophoneModel g)
            {
                _geophoneEntities[g].Visible = g.GIsVisible;
                GeophonesIsVisible = GeophoneModels.All(g => g.GIsVisible);
            }
            else
            {
                GeophonesIsVisible = !GeophonesIsVisible;
                foreach (KeyValuePair<GeophoneModel, Entity> pair in _geophoneEntities)
                    pair.Key.GIsVisible = pair.Value.Visible = GeophonesIsVisible;
            }
            Model.Invalidate();
        }


        private RelayCommand _geophoneSphereVisibilityCommand;
        /// <summary>
        /// Включение/отключение сфер чувствительности геофонов
        /// Если null, то это для всех сфер геофонов
        /// </summary>
        public ICommand GeophoneSphereVisibilityCommand
            => _geophoneSphereVisibilityCommand
            ?? (_geophoneSphereVisibilityCommand = new RelayCommand(ExecuteGeophoneSphereVisibilityCommand));

        private void ExecuteGeophoneSphereVisibilityCommand(object o)
        {
            if (o is GeophoneModel g)
            {
                _geophoneSphereEntities[g].Visible = g.SIsVisible;
                GeophoneSpheresIsVisible = GeophoneModels.All(g => g.SIsVisible);
            }
            else
            {
                GeophoneSpheresIsVisible = !GeophoneSpheresIsVisible;
                foreach (KeyValuePair<GeophoneModel, Entity> pair in _geophoneSphereEntities)
                    pair.Key.SIsVisible = pair.Value.Visible = GeophoneSpheresIsVisible;
            }
            Model.Invalidate();
        }

        private RelayCommand _changeColorCommand;
        public ICommand ChangeColorCommand
            => _changeColorCommand
            ?? (_changeColorCommand = new RelayCommand(ExecuteChangeColorCommand));

        private void ExecuteChangeColorCommand(object o)
        {
            if (SelectedGeophone is null)
            {
                GeophonesColor = SelectedColor;
                foreach (var item in GeophoneModels)
                {
                    item.Color = SelectedColor;
                    _geophoneEntities[item].Color = _geophoneSphereEntities[item].Color = ColorTranslator.FromHtml(SelectedColor);
                }
            }
            else
            {
                GeophonesColor = TRANSPARENT;
                SelectedGeophone.Color = SelectedColor;
                _geophoneEntities[SelectedGeophone].Color = ColorTranslator.FromHtml(SelectedColor);
                _geophoneSphereEntities[SelectedGeophone].Color = ColorTranslator.FromHtml(SelectedColor);
            }
            Model.Invalidate();
        }

        #endregion

        private RelayCommand _selectGeophoneCommand;
        public ICommand SelectGeophoneCommand
            => _selectGeophoneCommand
               ?? (_selectGeophoneCommand = new RelayCommand(ExecuteSelectGeophoneCommand));

        private void ExecuteSelectGeophoneCommand(object obj)
            => SelectedGeophone = obj as GeophoneModel;


        private RelayCommand _editGeophoneCommand;
        public ICommand EditGeophoneCommand
            => _editGeophoneCommand
               ?? (_editGeophoneCommand = new RelayCommand(ExecuteEditGeophoneCommand));

        private void ExecuteEditGeophoneCommand(object obj)
        {
            SelectedGeophone = obj as GeophoneModel;

            GeophoneViewModel = new GeophoneViewModel(_geophones[SelectedGeophone], "Редактировать геофон");

            GeophoneViewModel.Back += g => UpdateGeophoneCollection(g);
        }

        #endregion

        /// <summary>
        /// Добавление нового геофона в коллекцию или редактирование
        /// имеющегося в коллекции геофона;
        /// обновление изображения
        /// </summary>
        /// <param name="editedGeophone"></param>
        /// <param name="isNew">Явяется ли геофон новым</param>
        private void UpdateGeophoneCollection(Geophone editedGeophone)
        {
            if (editedGeophone is null)
                return;

            var meshG = GeophoneConversionService.CreateGeophoneEntity(editedGeophone);
            var meshS = GeophoneConversionService.CreateGeophoneSphereEntity(editedGeophone);

            var geophoneModel = _geophones.FirstOrDefault(pair => pair.Value == editedGeophone).Key;
            
            // Пользователь добавил новый геофон
            if (geophoneModel is null)
            {
                geophoneModel = GeophoneConversionService.GeophoneToGeophoneModel(editedGeophone);

                _geophones.Add(geophoneModel, editedGeophone);
                GeophoneModels.Add(geophoneModel);
                _geophoneEntities.Add(geophoneModel, meshG);
                _geophoneSphereEntities.Add(geophoneModel, meshS);
                EntityList.Add(meshG);
                EntityList.Add(meshS);
            }

            // Пользователь отредактировал геофон
            else
            {
                geophoneModel = GeophoneConversionService.Copy(geophoneModel, editedGeophone);

                EntityList.Remove(_geophoneEntities[geophoneModel]);
                EntityList.Remove(_geophoneSphereEntities[geophoneModel]);

                _geophoneEntities[geophoneModel] = meshG;
                _geophoneSphereEntities[geophoneModel] = meshS;
                EntityList.Add(meshG);
                EntityList.Add(meshS);
            }

            GeophonesIsVisible = _geophoneEntities.Keys.Any(g => g.GIsVisible);
            GeophoneSpheresIsVisible = _geophoneSphereEntities.Keys.Any(g => g.SIsVisible);
            
            Model.Invalidate();
        }


        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
