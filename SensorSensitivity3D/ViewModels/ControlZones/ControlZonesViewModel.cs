using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using GeometryGym.Ifc;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Enums;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using SensorSensitivity3D.ViewModels.Base;
using SensorSensitivity3D.Views;
using SensorSensitivity3D.Views.ControlZones;
using SensorSensitivity3D.Views.GeophoneUserControls;
using static SensorSensitivity3D.Services.ModelInteractionService;


namespace SensorSensitivity3D.ViewModels.ControlZones
{
    public class ControlZonesViewModel : BaseViewModel
    {
        private readonly Configuration _config;

        public event Action<Entity> SelectionEntity;

        private readonly ControlZoneService _zoneService;

        private readonly IEnumerable<GeophoneModel> Geophones;

        public ObservableCollection<ControlZoneModel> ControlZones { get; set; }

        public IEnumerable<SensitivityDomain> SensitivityDomains { get; set; }
        
        public string ControlZonesColor { get; set; }

        public const string TRANSPARENT = "Transparent";


        //private bool _controlZoneIsVisible;
        //public bool ControlZonesIsVisible
        //{
        //    get => ControlZones.Any(z => z.IsVisible);
        //    set
        //    {
        //        foreach (var z in ControlZones)
        //            z.IsVisible = value;

        //        UpdateVisibilityParams();
        //    }
        //}

        public bool ControlZonesIsVisible { get; set; }



        public bool ColorEditorIsOpen { get; set; }

        public ControlZoneModel SelectedControlZone { get; set; }

        public ControlZoneViewModel ControlZoneViewModel { get; set; }

        public ControlZonesViewModel() { }

        public ControlZonesViewModel(
            ControlZoneService zoneService, 
            Configuration config, 
            IEnumerable<GeophoneModel> geophones)
        {
            _zoneService = zoneService;
            _config = config;
            Geophones = geophones;

            ControlZones = new ObservableCollection<ControlZoneModel>
            (
                _zoneService.GetConfigControlZones(config.Id)
            );

            var entities = new Entity[ControlZones.Count];

            var i = 0;
            foreach (var z in ControlZones)
            {

                entities[i] = z.Entity;
                ++i;
            }

            AddEntities(entities);

            ControlZoneViewModel = new ControlZoneViewModel();
            ControlZoneViewModel.Back += ExecuteControlZoneOperation;

            UpdateControlZonesColor();
            UpdateVisibilityParams();
        }


        #region commands

        #region editing of control zone

        private RelayCommand _addControlZoneCommand;
        public ICommand AddControlZoneCommand
            => _addControlZoneCommand ??= new RelayCommand(ExecuteAddControlZoneCommand);

        public void ExecuteAddControlZoneCommand(object o)
        {
            ControlZoneViewModel.ActivateControlZoneViewModel(null);
        }

        private RelayCommand _editControlZoneCommand;
        public ICommand EditControlZoneCommand
            => _editControlZoneCommand ??= new RelayCommand(ExecuteEditControlZoneCommand);

        private void ExecuteEditControlZoneCommand(object obj)
        {
            ControlZoneViewModel.ActivateControlZoneViewModel(SelectedControlZone);
        }


        private RelayCommand _removeControlZoneCommand;
        public ICommand RemoveControlZoneCommand
            => _removeControlZoneCommand ??= new RelayCommand(ExecuteRemoveControlZoneCommand);

        private void ExecuteRemoveControlZoneCommand(object o)
        {
            if (_zoneService.RemoveControlZone(SelectedControlZone))
            {
                RemoveEntities(new [] {SelectedControlZone.Entity});
                ControlZones.Remove(SelectedControlZone);
            }

            //UpdateGeophonesColor();
            //UpdateVisibilityParams();
        }

        #endregion

        #region change visibility parameters of control zones

        private RelayCommand _changeControlZoneVisibilityCommand;
        public ICommand ChangeControlZoneVisibilityCommand
            => _changeControlZoneVisibilityCommand ??= new RelayCommand(ExecuteChangeControlZoneVisibilityCommand);

        private void ExecuteChangeControlZoneVisibilityCommand(object o)
        {
            if (SelectedControlZone is null)
            {
                foreach (var z in ControlZones)
                    z.IsVisible = ControlZonesIsVisible;
            }

            UpdateVisibilityParams();
        }

        
        private RelayCommand _changeControlZoneColorCommand;
        public ICommand ChangeControlZoneColorCommand
            => _changeControlZoneColorCommand ??= new RelayCommand(ExecuteChangeControlZoneColorCommand);

        private void ExecuteChangeControlZoneColorCommand(object o)
        {
            // снимаем выделение, чтобы было видно изменения
            if (SelectedControlZone != null)
                SelectionEntity?.Invoke(null);

            var color = o.ToString();

            if (SelectedControlZone is null)
            {
                ControlZonesColor = color;
                foreach (var g in ControlZones)
                    g.Color = color;
            }
            else
            {
                SelectedControlZone.Color = color;
                UpdateControlZonesColor();
            }

            UpdateVisibility();
        }


        private RelayCommand _resetControlZoneCommand;
        public ICommand ResetControlZoneCommand
            => _resetControlZoneCommand ??= new RelayCommand(ExecuteResetControlZoneCommand, CanExecuteResetControlZoneCommand);

        private void ExecuteResetControlZoneCommand(object obj)
        {
            IList<Entity> oldEntities, newEntities;

            // Откат к сохраненным настройкам
            // зон контроля
            if (SelectedControlZone is null)
            {
                var changedZones = ControlZones.Where(g => g.IsChanged);

                var count = changedZones.Count();

                oldEntities = new Entity[count];
                newEntities = new Entity[count];

                var i = 0;
                foreach (var z in changedZones)
                {
                    oldEntities[i] = z.Entity;

                    z.ResetControlZoneSettings();

                    newEntities[i] = z.Entity;

                    ++i;
                }
            }
            // зоны контроля
            else
            {
                oldEntities = new[] {SelectedControlZone.Entity}.Select(e => e).ToList();

                SelectedControlZone.ResetControlZoneSettings();

                newEntities = new []{SelectedControlZone.Entity};
            }

            ReplaceEntities(oldEntities, newEntities);

            UpdateControlZonesColor();
            UpdateVisibilityParams();
        }

        private bool CanExecuteResetControlZoneCommand(object obj)
            => obj is ControlZoneModel controlZoneModel
                ? controlZoneModel.IsChanged
                : ControlZones?.Any(g => g.IsChanged) ?? false;

        #endregion

        #region additional commands

        private RelayCommand _calcControlZoneCommand;
        public ICommand CalcControlZoneCommand
            => _calcControlZoneCommand ??= new RelayCommand(ExecuteCalcControlZoneCommand);

        private void ExecuteCalcControlZoneCommand(object obj)
        {
            var tempZone = SelectedControlZone;

            var accept = true;
            if (Geophones.Any(g => g.IsChanged))
                accept = MessageBox.Show(
                    "Нажмите Ок для сохранения параметров геофонов",
                    "Геофоны были изменены",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information) == MessageBoxResult.OK;

            if (!accept)
                return;
            
            // все зоны контроля
            if (tempZone is null)
            {
                //foreach (var zone in ControlZones)
                //{
                //    _zoneService.CalculationSensitivityZones(zone, Geophones);
                //    zone.IsCalculated = true;
                //}
                    
            }
            // одну зону контроля
            else
            {
                IEnumerable<SensitivityZone> sensitivityZones = _zoneService.CalculationSensitivityZones(tempZone.GeometricZone, Geophones);
                tempZone.IsCalculated = true;

                if (sensitivityZones.Count() == 0)
                    return;


                double volume = sensitivityZones.Sum(z => z.Volume);
                tempZone.K = volume / tempZone.GeometricZone.Volume * 100;
                tempZone.SMin = sensitivityZones.Min(z => z.SMin);
                tempZone.SMax = sensitivityZones.Max(z => z.SMax);

                foreach (var zone in sensitivityZones)
                {
                    zone.Body = UtilityEx.ConvexHull(zone.Domains
                        .Select(d => new Point3D(d.X, d.Y, d.Z))
                        .ToList());
                    zone.Body.ColorMethod = colorMethodType.byEntity;
                    zone.Body.Color = Color.IndianRed;
                }
                AddEntities(sensitivityZones.Select(z => z.Body));


                _zoneService.SaveControlZone(tempZone);
            }
        }

        private RelayCommand _selectControlZoneCommand;
        public ICommand SelectControlZoneCommand
            => _selectControlZoneCommand ??= new RelayCommand(ExecuteSelectControlZoneCommand);

        private void ExecuteSelectControlZoneCommand(object obj)
        {
            SelectedControlZone = obj as ControlZoneModel;

            SelectionEntity?.Invoke(SelectedControlZone?.Entity);
        }


        private RelayCommand _goToControlZoneCommand;
        public ICommand GoToControlZoneCommand
            => _goToControlZoneCommand ??= new RelayCommand(ExecuteGoToControlZoneCommand);

        private void ExecuteGoToControlZoneCommand(object o)
        {
            ZoomFitEntities(new []{SelectedControlZone.Entity});
    }

        #endregion

        #region saving and loading control zone
        
        private RelayCommand _loadControlZonesFromDbCommand;
        public ICommand LoadControlZonesFromDbCommand
            => _loadControlZonesFromDbCommand ??= new RelayCommand(ExecuteLoadControlZonesFromDbCommand);

        private void ExecuteLoadControlZonesFromDbCommand(object obj)
        {
            var connectionViewModel = new DbConnectViewModel();

            var connectionWindow = new DbConnectWindow
            {
                DataContext = connectionViewModel
            };

            connectionViewModel.OnSuccessConnection += connectionString =>
            {
                var loadedZones = _zoneService.GetZonesFromGCSDb(connectionString, out string errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!loadedZones.Any())
                {
                    MessageBox.Show(
                        "В выбранной базе данных не обнаружено пространственных зон",
                        "Сообщение", MessageBoxButton.OK);
                    return;
                }

                connectionWindow.Close();

                var editorWindow = new ControlZonesEditor("Загрузка пространственных зон из базы данных Geoacoustics");
                var editorViewModel = new ControlZonesEditorViewModel(loadedZones, isFull: false);
                editorViewModel.OnSelectionZones += selectedZones =>
                {
                    AddGeophones(selectedZones);
                    editorWindow.Close();
                };

                editorWindow.DataContext = editorViewModel;
                editorWindow.ShowDialog();
            };

            connectionWindow.ShowDialog();
        }

        #endregion

        #endregion

        /// <summary>
        /// Добавление нескольких зон в БД и обновление 
        /// отображения зон
        /// </summary>
        /// <param name="zones"></param>
        private void AddGeophones(IEnumerable<ControlZoneModel> zones)
        {
            var before = ControlZones.Count;

            foreach (var z in zones)
            {
                z.InitEntity();
                if (_zoneService.AddControlZone(z, _config.Id))
                    ControlZones.Add(z);
            }

            MessageBox.Show($"Успешно добавлено пространственных зон - {ControlZones.Count - before}");

            AddEntities(zones.Select(z => z.Entity));
            UpdateControlZonesColor();
            UpdateVisibilityParams();
        }


        /// <summary>
        /// Добавление новой зоны контроля в коллекцию или редактирование
        /// </summary>
        /// <param name="editedControlZone"></param>
        private void ExecuteControlZoneOperation(ModelOperation operation, ControlZoneModel editedControlZone)
        {
            switch (operation)
            {
                case ModelOperation.Add:
                    if (_zoneService.AddControlZone(editedControlZone, _config.Id))
                        ControlZones.Add(editedControlZone);
                    break;
                case ModelOperation.AddAndContinueAdding:
                    if (_zoneService.AddControlZone(editedControlZone, _config.Id))
                    {
                        ControlZones.Add(editedControlZone);
                        ControlZoneViewModel.ActivateControlZoneViewModel(editedControlZone);
                    }
                    break;
                case ModelOperation.Edit:
                {
                    editedControlZone.IsCalculated = false;
                    _zoneService.SaveControlZone(editedControlZone);
                    break;

                }
                default:
                    return;
            }
            UpdateControlZonesColor();
            UpdateVisibilityParams();
        }

        private void UpdateControlZonesColor()
        {
            var firstZone = ControlZones.FirstOrDefault();
            ControlZonesColor =
                firstZone != null && ControlZones.Skip(1).All(g => g.Color == firstZone.Color)
                    ? firstZone.Color
                    : TRANSPARENT;
        }

        private void UpdateVisibilityParams()
        {
            if (!ControlZones.Any())
                return;

            ControlZonesIsVisible = ControlZones.All(z => z.IsVisible);

            UpdateVisibility();
        }

        public string TrySelectControlZone()
        {
            SelectedControlZone = ControlZones.FirstOrDefault(z => z.Entity.Selected);
            return SelectedControlZone?.ToString();
        }


        protected override void OnDispose()
        {
            ControlZones?.Clear();
        }
    }
}