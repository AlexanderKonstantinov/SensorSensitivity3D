using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using devDept.Eyeshot.Entities;
using devDept.Graphics;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using SensorSensitivity3D.ViewModels.Base;
using SensorSensitivity3D.ViewModels.GeophoneViewModels;

using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ConfigService _configService;
        private readonly GeophoneService _geophoneService;

        public bool ConfigPanelVisibility { get; set; } = true;

        public string SelectedEntityInfo { get; set; }


        public DrawingViewModel DrawingViewModel { get; set; }
        public GeophonesViewModel GeophonesViewModel { get; set; }


        public ObservableCollection<Configuration> Configurations { get; set; }
        public Configuration SelectedConfig { get; set; }
        

        public MainViewModel() { }

        public MainViewModel(CustomModel model)
        {
            _configService = new ConfigService();
            _geophoneService = new GeophoneService();

            Configurations = _configService.GetConfigurations();

            ModelInteractionService.Init(model);
            OnPropertyChanged("ViewportEntities");

            model.MouseMove += (o, a) =>
            {
                var selectedEntity = model.Entities.ElementAtOrDefault
                        (model.GetEntityUnderMouseCursor(RenderContextUtility.ConvertPoint(model.GetMousePosition(a))));
                
                SelectEntity(selectedEntity);

                SelectedEntityInfo = GeophonesViewModel?.TrySelectGeophone();                
            };
        }

        #region Commands

        private RelayCommand _loadConfigCommand;
        public ICommand LoadConfigCommand
            => _loadConfigCommand ??= new RelayCommand(ExecuteLoadConfigCommand, null);
        
        private void ExecuteLoadConfigCommand(object obj)
        {
            MessageBox.Show("Success");

            SelectedConfig = obj as Configuration;
                
            DrawingViewModel = new DrawingViewModel(_configService, SelectedConfig);

            GeophonesViewModel = new GeophonesViewModel(_geophoneService, SelectedConfig);
                       

            GeophonesViewModel.SelectionEntities += entities 
                => OnSelectionEntities(entities);
                        
            ConfigPanelVisibility = false;

            Focus();
            ZoomFit();
        }


        private RelayCommand _selectConfigCommand;
        public ICommand SelectConfigCommand
            => _selectConfigCommand ??= new RelayCommand(ExecuteSelectConfigCommand);

        private void ExecuteSelectConfigCommand(object obj)
        {
            SelectedConfig = obj as Configuration;
            OnPropertyChanged(nameof(SelectedConfig));
        }


        private RelayCommand _zoomFitEntityCommand;
        public ICommand ZoomFitEntityCommand
            => _zoomFitEntityCommand ??= new RelayCommand(ExecuteZoomFitEntityCommand);

        private void ExecuteZoomFitEntityCommand(object obj)
        {
            ZoomFitEntities();
        }              


        public void SaveConfig(bool isShowWarning)
        {
            if (GeophonesViewModel.GeophoneModels.Any(g => g.IsChanged))
            {
                if (isShowWarning)
                {
                    var result = MessageBox.Show("Желаете сохранить внесенные изменения", "Геофоны были изменены", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                    
                    if (result == MessageBoxResult.Cancel)
                        return;
                    
                    if (result == MessageBoxResult.No)
                    {
                        ModelClear();
                        ConfigPanelVisibility = true;
                        ExecuteSelectConfigCommand(null);
                    }
                }

                _geophoneService.SaveGeophones(GeophonesViewModel.GeophoneModels);
            }
        }


        #region edit configs

        private RelayCommand _addConfigCommand;
        public ICommand AddConfigCommand
            => _addConfigCommand ??= new RelayCommand(ExecuteAddConfigCommand, CanExecuteAddConfigCommand);

        private void ExecuteAddConfigCommand(object obj)
        {
            var newConfig = new Configuration { Name = obj.ToString() };
            if (_configService.AddConfiguration(newConfig))
                Configurations.Add(newConfig);
        }

        private bool CanExecuteAddConfigCommand(object obj)
            => (obj is string name)
            && !string.IsNullOrEmpty(name)
            && Configurations.All(c => c.Name != name);


        private RelayCommand _removeConfigCommand;
        public ICommand RemoveConfigCommand
            => _removeConfigCommand ??= new RelayCommand(ExecuteRemoveConfigCommand);

        private void ExecuteRemoveConfigCommand(object obj)
        {
            var res = MessageBox.Show(
                "Вы уверены, что хотите удалить конфигурацию со всеми настройками и геофонами",
                "Подтверждение",
                MessageBoxButton.OKCancel);

            if (res != MessageBoxResult.OK)
                return;

            var removedConfig = (Configuration)obj;
            if (_configService.RemoveConfiguration(removedConfig))
                Configurations.Remove(removedConfig);
        }


        private RelayCommand _editConfigCommand;
        public ICommand EditConfigCommand
            => _editConfigCommand ??= new RelayCommand(ExecuteEditConfigCommand, CanExecuteEditConfigCommand);

        private void ExecuteEditConfigCommand(object obj)
        {
            SelectedConfig.Name = obj.ToString();
            _configService.EditConfiguration(SelectedConfig);
        }

        private bool CanExecuteEditConfigCommand(object obj)
            => (obj is string editedName)
            && !string.IsNullOrEmpty(editedName)
            && Configurations != null
            && !Configurations.Any(c => c.Name.Equals(editedName));

        
        #endregion

        #endregion

        private void OnSelectionEntities(IEnumerable<Entity> entities)
        {            
            SelectEntities(entities);
            SelectedEntityInfo = null;
        }

        protected override void OnDispose()
        {
            Configurations?.Clear();
        }
    }
}
