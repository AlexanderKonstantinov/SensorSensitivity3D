using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using devDept.Eyeshot;
using devDept.Graphics;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using SensorSensitivity3D.ViewModels.GeophoneViewModels;

using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ConfigService _configService;
        private readonly GeophoneService _geophoneService;

        public Visibility RightPanelVisibility { get; set; } = Visibility.Visible;

        public static string SelectedEntityInfo { get; set; }


        public DrawingViewModel DrawingViewModel { get; set; }
        public GeophonesViewModel GeophonesViewModel { get; set; }
        public GeophoneViewModel GeophoneViewModel { get; set; }


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
                SelectEntity(
                    model.Entities.ElementAtOrDefault
                        (model.GetEntityUnderMouseCursor(RenderContextUtility.ConvertPoint(model.GetMousePosition(a)))));
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

            DrawingViewModel = new DrawingViewModel(_configService, SelectedConfig);

            GeophonesViewModel = new GeophonesViewModel(_geophoneService, SelectedConfig);
                                   
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
