using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
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
        private readonly ConfigService _configService;

        public Visibility RightPanelVisibility { get; set; } = Visibility.Visible;


        public DrawingViewModel DrawingViewModel { get; set; }
        public GeophonesViewModel GeophonesViewModel { get; set; }
        public GeophoneViewModel GeophoneViewModel { get; set; }


        public ObservableCollection<Configuration> Configurations { get; set; }
        public Configuration SelectedConfig { get; set; }
                        
        
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

        private int _selectedEntityIndex;
        public string SelectedEntityInfo { get; set; }


        public MainViewModel() { }

        
        public MainViewModel(CustomModel model)
        {
            _configService = new ConfigService();

            Configurations = _configService.GetConfigurations();

            _model = model;
            _entityList = new CustomEntityList();

            _selectedEntityIndex = -1;

            _model.MouseMove += (o, a) =>
            {
                if (_selectedEntityIndex >= 0 && _selectedEntityIndex < _model.Entities.Count)
                    _model.Entities[_selectedEntityIndex].Selected = false;
                
                var position = a.GetPosition(_model);

                _selectedEntityIndex = _model.GetEntityUnderMouseCursor(new Point((int) position.X, (int) position.Y));
                
                if (_selectedEntityIndex == -1)
                {
                    SelectedEntityInfo = null;
                }
                else if (_model.Entities[_selectedEntityIndex].Selectable)
                {
                    _model.Entities[_selectedEntityIndex].Selected = true;
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
            Configurations?.Clear();
        }
    }
}
