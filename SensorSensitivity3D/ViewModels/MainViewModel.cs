using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using devDept.Eyeshot;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Helpers;
using Telerik.Windows.Controls;

namespace SensorSensitivity3D.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public Visibility RightPanelVisibility { get; set; } = Visibility.Visible;

        public Configuration Config { get; set; }
        
        public ModelViewModel ModelViewModel { get; set; }
        public DrawingViewModel DrawingViewModel { get; set; }

        public readonly CustomModel Model;


        public static ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>
        {
            new Sensor
            {
                HoleNumber = 1111,
                X = 123123,
                Y = 123123,
                Z = 123123,
                IsActive = true,
                SColor = "#ffffff",
                SIsVisible = true,
                R = 100,
                RColor = "#ffffff",
                RIsVisible = true
            },
            new Sensor
            {
                HoleNumber = 1111,
                X = 123123,
                Y = 123123,
                Z = 123123,
                IsActive = true,
                SColor = "#ffffff",
                SIsVisible = true,
                R = 100,
                RColor = "#ffffff",
                RIsVisible = true
            },
            new Sensor
            {
                HoleNumber = 1111,
                X = 123123,
                Y = 123123,
                Z = 123123,
                IsActive = true,
                SColor = "#ffffff",
                SIsVisible = true,
                R = 100,
                RColor = "#ffffff",
                RIsVisible = true
            }
        };

        public MainViewModel() { }
        public MainViewModel(CustomModel model)
        {
            Debug();

            Model = model;

            DrawingViewModel = new DrawingViewModel(Config.Drawing, Model);
            ModelViewModel = new ModelViewModel(Model, Config.Drawing);
            
            
            DrawingViewModel.DrawingLoaded += path 
                => ModelViewModel.Redraw(path);
        }

        private void Debug()
        {
            Config = new Configuration
            {
                //Drawing = new Drawing()
                Drawing = new Drawing(@"G:\Pioner.dwg")
            };
        }

        #region Commands

        private RelayCommand _loadCommand;
        public ICommand LoadCommand
            => _loadCommand
               ?? (_loadCommand = new RelayCommand(ExecuteLoadCommand, null));
        
        private void ExecuteLoadCommand(object obj)
        {

        }


        private RelayCommand _panelCollapseCommand;
        public ICommand PanelCollapseCommand
            => _panelCollapseCommand
               ?? (_panelCollapseCommand = new RelayCommand(ExecutePanelCollapseCommand, CanExecutePanelCollapseCommand));

        private void ExecutePanelCollapseCommand(object obj)
        {
            RightPanelVisibility = RightPanelVisibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private bool CanExecutePanelCollapseCommand(object o)
            => true;

        #endregion

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
