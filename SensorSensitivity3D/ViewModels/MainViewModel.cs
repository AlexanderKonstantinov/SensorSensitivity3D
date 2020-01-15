using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using SensorSensitivity3D.ViewModels.GeophoneViewModels;
using Point = System.Drawing.Point;

namespace SensorSensitivity3D.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private int _curSelectedEntityIndex;
        public string SelectedEntityInfo { get; set; }
        //private ToolTip _entityToolTip = new ToolTip{ Style = Telerik};

        private readonly ConfigService _configService;

        public ObservableCollection<Configuration> Configurations { get; set; }
        
        private readonly CustomModel _model;

        private CustomEntityList _entityList;
        public CustomEntityList EntityList
        {
            get => _entityList;
            set
            {
                _entityList = value;
                OnPropertyChanged("ViewportEntities");
            }
        }

        public Visibility RightPanelVisibility { get; set; } = Visibility.Visible;

        public Configuration SelectedConfig { get; set; }

        public DrawingViewModel DrawingViewModel { get; set; }

        public GeophonesViewModel GeophonesViewModel { get; set; }
        public GeophoneViewModel GeophoneViewModel { get; set; }


        public MainViewModel() { }

        

        public MainViewModel(CustomModel model)
        {
            _configService = new ConfigService();

            Configurations = _configService.GetConfigurations();

            _model = model;
            _entityList = new CustomEntityList();

            _model.MouseMove += (o, a) =>
            {
                if (_curSelectedEntityIndex >= 0 && _curSelectedEntityIndex < _model.Entities.Count)
                    _model.Entities[_curSelectedEntityIndex].Selected = false;
                
                var position = a.GetPosition(_model);

                _curSelectedEntityIndex = _model.GetEntityUnderMouseCursor(new Point((int) position.X, (int) position.Y));
                
                if (_curSelectedEntityIndex == -1)
                {
                    SelectedEntityInfo = null;
                }
                else
                {
                    _model.Entities[_curSelectedEntityIndex].Selected = true;
                    SelectedEntityInfo = GeophonesViewModel.TrySelectedGeophone()?.ToString();
                }

                _model.Invalidate();
            };
        }

        #region Commands


        private RelayCommand _loadCommand;
        public ICommand LoadCommand
            => _loadCommand ??= new RelayCommand(ExecuteLoadCommand, null);
        
        private void ExecuteLoadCommand(object obj)
        {

        }


        private RelayCommand _selectConfigCommand;
        public ICommand SelectConfigCommand
            => _selectConfigCommand ??= new RelayCommand(ExecuteSelectConfigCommand);

        private void ExecuteSelectConfigCommand(object obj)
        {
            SelectedConfig = obj as Configuration;

            DrawingViewModel = new DrawingViewModel(_model, SelectedConfig);

            GeophonesViewModel = new GeophonesViewModel(
                _model,
                EntityList,
                SelectedConfig);
                                   
            _model.ZoomFit();
            _model.Invalidate();
        }


        private RelayCommand _panelCollapseCommand;
        public ICommand PanelCollapseCommand
            => _panelCollapseCommand ??= new RelayCommand(ExecutePanelCollapseCommand);

        private void ExecutePanelCollapseCommand(object obj)
        {
            RightPanelVisibility = RightPanelVisibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        #endregion


        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
