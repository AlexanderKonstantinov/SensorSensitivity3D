﻿using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Enums;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using SensorSensitivity3D.ViewModels.Base;
using SensorSensitivity3D.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static SensorSensitivity3D.Services.ModelInteractionService;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophonesViewModel : BaseViewModel
    {
        private readonly Configuration _config;

        public event Action<IEnumerable<Entity>> SelectionEntities;

        private readonly GeophoneService _geophoneService;

        public ObservableCollection<GeophoneModel> GeophoneModels { get; set; }

        public const string TRANSPARENT = "Transparent";

        public string GeophonesColor { get; set; }


        public bool GeophonesIsVisible
        {
            get => GeophoneModels.Any(g => g.GIsVisible || g.SIsVisible);
            set
            {
                foreach (var g in GeophoneModels)
                    g.GIsVisible = g.SIsVisible = value;

                UpdateVisibilityParams();
            }
        }

        public bool GeophoneCentersIsVisible { get; set; }
        public bool GeophoneSensitivitySpheresIsVisible { get; set; }



        public bool ColorEditorIsOpen { get; set; }
        public GeophoneModel SelectedGeophone { get; set; }

        public GeophoneViewModel GeophoneViewModel { get; set; }


        public GeophonesViewModel() { }
        public GeophonesViewModel(GeophoneService geophoneService, Configuration config)
        {
            _config = config;
            _geophoneService = geophoneService;

            GeophoneModels = new ObservableCollection<GeophoneModel>
            (
                _geophoneService.GetConfigGeophones(config.Id)
            );

            var entities = new Entity[GeophoneModels.Count * 2];

            var i = 0;
            foreach (var g in GeophoneModels)
            {
                entities[i] = g.CenterSphere;
                entities[i + 1] = g.SensitivitySphere;

                i += 2;
            }

            AddEntities(entities);

            GeophoneViewModel = new GeophoneViewModel();
            GeophoneViewModel.Back += ExecuteGeophoneOperation;

            UpdateGeophonesColor();

            UpdateVisibilityParams();
        }

        #region commands

        #region editing of geophones

        private RelayCommand _addGeophoneCommand;
        public ICommand AddGeophoneCommand
            => _addGeophoneCommand ??= new RelayCommand(ExecuteAddGeophoneCommand);

        public void ExecuteAddGeophoneCommand(object o)
        {
            GeophoneViewModel.ActivateGeophoneViewModel(null);
        }

        private RelayCommand _editGeophoneCommand;
        public ICommand EditGeophoneCommand
            => _editGeophoneCommand ??= new RelayCommand(ExecuteEditGeophoneCommand);

        private void ExecuteEditGeophoneCommand(object obj)
        {
            GeophoneViewModel.ActivateGeophoneViewModel(SelectedGeophone);
        }


        private RelayCommand _removeGeophoneCommand;
        public ICommand RemoveGeophoneCommand
            => _removeGeophoneCommand ??= new RelayCommand(ExecuteRemoveGeophoneCommand);

        private void ExecuteRemoveGeophoneCommand(object o)
        {
            if (_geophoneService.RemoveGeophone(SelectedGeophone))
            {
                RemoveEntities(SelectedGeophone.Entities);
                GeophoneModels.Remove(SelectedGeophone);
            }

            UpdateGeophonesColor();
            UpdateVisibilityParams();
        }

        #endregion

        #region change visibility parameters of geophones

        private RelayCommand _changeGeophoneCenterVisibility;
        public ICommand ChangeGeophoneCenterVisibilityCommand
            => _changeGeophoneCenterVisibility ??= new RelayCommand(ExecuteChangeGeophoneCenterVisibilityCommand);

        private void ExecuteChangeGeophoneCenterVisibilityCommand(object o)
        {
            if (SelectedGeophone is null)
            {
                foreach (var g in GeophoneModels)
                    g.GIsVisible = GeophoneCentersIsVisible;
            }

            UpdateVisibilityParams();
        }


        private RelayCommand _changeSensitivitySphereVisibility;
        public ICommand ChangeSensitivitySphereVisibilityCommand
            => _changeSensitivitySphereVisibility ??= new RelayCommand(ExecuteChangeSensitivitySphereVisibilityCommand);

        private void ExecuteChangeSensitivitySphereVisibilityCommand(object o)
        {
            if (SelectedGeophone is null)
            {
                foreach (var g in GeophoneModels)
                    g.SIsVisible = GeophoneSensitivitySpheresIsVisible;
            }

            UpdateVisibilityParams();
        }


        private RelayCommand _changeColorCommand;
        public ICommand ChangeColorCommand
            => _changeColorCommand ??= new RelayCommand(ExecuteChangeColorCommand);

        private void ExecuteChangeColorCommand(object o)
        {
            // снимаем выделение, чтобы было видно изменения
            if (SelectedGeophone != null)
                SelectionEntities?.Invoke(null);

            var color = o.ToString();

            if (SelectedGeophone is null)
            {
                GeophonesColor = color;
                foreach (var g in GeophoneModels)
                    g.Color = color;
            }
            else
            {
                SelectedGeophone.Color = color;
                UpdateGeophonesColor();
            }

            UpdateVisibility();
        }


        private RelayCommand _resetGeophoneCommandCommand;
        public ICommand ResetGeophoneCommand
            => _resetGeophoneCommandCommand ??= new RelayCommand(ExecuteResetGeophoneCommand, CanExecuteResetGeophoneCommandCommand);

        private void ExecuteResetGeophoneCommand(object obj)
        {
            IList<Entity> oldEntities, newEntities;

            // Откат к сохраненным настройкам
            // геофонов
            if (SelectedGeophone is null)
            {
                var changedGeophones = GeophoneModels.Where(g => g.IsChanged);

                var count = changedGeophones.Count();

                oldEntities = new Entity[count * 2];
                newEntities = new Entity[count * 2];

                var i = 0;
                foreach (var g in changedGeophones)
                {
                    oldEntities[i] = g.CenterSphere;
                    oldEntities[i + 1] = g.SensitivitySphere;

                    g.ResetGeophoneSettings();

                    newEntities[i] = g.CenterSphere;
                    newEntities[i + 1] = g.SensitivitySphere;

                    i += 2;
                }
            }
            // геофона
            else
            {
                oldEntities = SelectedGeophone.Entities.Select(e => e).ToList();

                SelectedGeophone.ResetGeophoneSettings();

                newEntities = SelectedGeophone.Entities;
            }

            ReplaceEntities(oldEntities, newEntities);

            UpdateGeophonesColor();
            UpdateVisibilityParams();
        }

        private bool CanExecuteResetGeophoneCommandCommand(object obj)
            => obj is GeophoneModel geophoneModel
                ? geophoneModel.IsChanged
                : GeophoneModels?.Any(g => g.IsChanged) ?? false;

        #endregion

        #region additional commands

        private RelayCommand _selectGeophoneCommand;
        public ICommand SelectGeophoneCommand
            => _selectGeophoneCommand ??= new RelayCommand(ExecuteSelectGeophoneCommand);

        private void ExecuteSelectGeophoneCommand(object obj)
        {
            SelectedGeophone = obj as GeophoneModel;

            SelectionEntities?.Invoke(SelectedGeophone?.Entities);
        }


        private RelayCommand _goToGeophoneCommand;
        public ICommand GoToGeophoneCommand
            => _goToGeophoneCommand ??= new RelayCommand(ExecuteGoToGeophoneCommand);

        private void ExecuteGoToGeophoneCommand(object o)
        {
            ZoomFitEntities(SelectedGeophone.Entities);
        }

        #endregion

        #region saving and loading geophones

        private RelayCommand _saveToFileCommand;
        public ICommand SaveToFileCommand
            => _saveToFileCommand ??= new RelayCommand(ExecuteSaveToFileCommand, CanExecuteSaveToFileCommand);

        private void ExecuteSaveToFileCommand(object obj)
        {
            var editorWindow = new GeophonesEditorWindow("Сохранение геофонов в файл");
            var editorViewModel = new GeophonesEditorViewModel(GeophoneModels, isFull: true);
            editorViewModel.OnSelectionGeophones += (selectedGeophones) =>
            {
                editorWindow.Close();

                var resourcesPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", _config.Name);

                var saveFileDialog = new SaveFileDialog
                {
                    FileName = $"Геофоны. {_config.Name}.xml",
                    InitialDirectory = Directory.Exists(resourcesPath) ? resourcesPath : Environment.CurrentDirectory,
                    AddExtension = true,
                    Title = "Выберите файл для сохранения",
                    Filter = "XML Format (*.xml)|*.xml"
                };

                if (saveFileDialog.ShowDialog() != true) return;

                var message = _geophoneService.SaveToFile(selectedGeophones, saveFileDialog.FileName)
                    ? "Геофоны успешно сохранены"
                    : "Ошибка при сохранении геофонов";

                System.Windows.MessageBox.Show(message);
            };

            editorWindow.DataContext = editorViewModel;
            editorWindow.ShowDialog();
        }

        private bool CanExecuteSaveToFileCommand(object obj)
            => GeophoneModels?.Any() ?? false;


        private RelayCommand _loadFromDbCommand;
        public ICommand LoadFromDbCommand
            => _loadFromDbCommand ??= new RelayCommand(ExecuteLoadFromDbCommand);

        private void ExecuteLoadFromDbCommand(object obj)
        {
            var connectionViewModel = new DbConnectViewModel();

            var connectionWindow = new DbConnectWindow
            {
                DataContext = connectionViewModel
            };

            connectionViewModel.OnSuccessConnection += connectionString =>
                {
                    var loadedGeophones = _geophoneService.GetGeophonesFromGCSDb(connectionString, out string errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!loadedGeophones.Any())
                    {
                        MessageBox.Show(
                            "В выбранной базе данных не обнаружено установленных геофонов",
                            "Сообщение", MessageBoxButton.OK);
                        return;
                    }

                    connectionWindow.Close();

                    var editorWindow = new GeophonesEditorWindow("Загрузка геофонов из базы данных Geoacoustics");
                    var editorViewModel = new GeophonesEditorViewModel(loadedGeophones, isFull: false);
                    editorViewModel.OnSelectionGeophones += selectedGeophones =>
                    {
                        AddGeophones(selectedGeophones);
                        editorWindow.Close();
                    };

                    editorWindow.DataContext = editorViewModel;
                    editorWindow.ShowDialog();
                };

            connectionWindow.ShowDialog();
        }


        private RelayCommand _loadFromFileCommand;
        public ICommand LoadFromFileCommand
            => _loadFromFileCommand ??= new RelayCommand(ExecuteLoadFromFileCommand);

        private void ExecuteLoadFromFileCommand(object obj)
        {
            var resourcesPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Resources", _config.Name);

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.Exists(resourcesPath) ? resourcesPath : Environment.CurrentDirectory,
                Title = "Выберите файл для загрузки",
                Filter = "XML Format (*.xml)|*.xml"
            };

            if (openFileDialog.ShowDialog() != true) return;

            var loadedGeophones = _geophoneService.LoadFromFile(openFileDialog.FileName);

            var editorWindow = new GeophonesEditorWindow("Загрузка геофонов из файла");
            var editorViewModel = new GeophonesEditorViewModel(loadedGeophones, isFull: true);
            editorViewModel.OnSelectionGeophones += (selectedGeophones) =>
            {
                AddGeophones(selectedGeophones);
                editorWindow.Close();
            };

            editorWindow.DataContext = editorViewModel;
            editorWindow.ShowDialog();
        }

        #endregion

        #endregion

        /// <summary>
        /// Добавление нескольких геофонов в БД и обновление 
        /// отображения геофонов
        /// </summary>
        /// <param name="geophones"></param>
        private void AddGeophones(IEnumerable<GeophoneModel> geophones)
        {
            var before = GeophoneModels.Count;

            foreach (var g in geophones)
            {
                g.InitEntities();
                if (_geophoneService.AddGeophone(g, _config.Id))
                    GeophoneModels.Add(g);
            }

            MessageBox.Show($"Успешно добавлено геофонов - {GeophoneModels.Count - before}");

            AddEntities(geophones.SelectMany(g => g.Entities));
            UpdateGeophonesColor();
            UpdateVisibilityParams();
        }


        /// <summary>
        /// Добавление нового геофона в коллекцию или редактирование
        /// имеющегося в коллекции геофона;
        /// обновление изображения
        /// </summary>
        /// <param name="editedGeophone"></param>
        private void ExecuteGeophoneOperation(GeophoneOperation operation, GeophoneModel editedGeophone)
        {
            switch (operation)
            {
                case GeophoneOperation.Add:
                    if (_geophoneService.AddGeophone(editedGeophone, _config.Id))
                        GeophoneModels.Add(editedGeophone);
                    break;
                case GeophoneOperation.AddAndContinueAdding:
                    if (_geophoneService.AddGeophone(editedGeophone, _config.Id))
                    {
                        GeophoneModels.Add(editedGeophone);
                        GeophoneViewModel.ActivateGeophoneViewModel(editedGeophone);
                    }
                    break;
                case GeophoneOperation.Edit:
                    {
                        _geophoneService.SaveGeophone(editedGeophone);
                    }
                    break;
                default:
                    return;
            }
            UpdateGeophonesColor();
            UpdateVisibilityParams();
        }

        private void UpdateGeophonesColor()
        {
            var firstGeophone = GeophoneModels.FirstOrDefault();
            GeophonesColor =
                firstGeophone != null && GeophoneModels.Skip(1).All(g => g.Color == firstGeophone.Color)
                ? firstGeophone.Color
                : TRANSPARENT;
        }

        private void UpdateVisibilityParams()
        {
            if (!GeophoneModels.Any())
                return;

            GeophoneCentersIsVisible = GeophoneModels.All(g => g.GIsVisible);
            GeophoneSensitivitySpheresIsVisible = GeophoneModels.All(g => g.SIsVisible);
            UpdateVisibility();

            OnPropertyChanged(nameof(GeophonesIsVisible));
        }

        public string TrySelectGeophone()
        {
            SelectedGeophone = GeophoneModels.FirstOrDefault(g => g.Entities.Any(e => e.Selected));
            return SelectedGeophone?.ToString();
        }

        protected override void OnDispose()
        {
            GeophoneModels?.Clear();
        }
    }
}
