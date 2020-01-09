using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.GeophoneViewModels;

namespace SensorSensitivity3D.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Random rnd = new Random();

        private readonly CustomModel Model;

        private CustomEntityList _entityList;
        public CustomEntityList EntityList
        {
            get { return _entityList; }
            set
            {
                _entityList = value;
                OnPropertyChanged("ViewportEntities");
            }
        }

        public Visibility RightPanelVisibility { get; set; } = Visibility.Visible;

        public Configuration Config { get; set; }

        public DrawingViewModel DrawingViewModel { get; set; }

        public GeophonesViewModel GeophonesViewModel { get; set; }
        public GeophoneViewModel GeophoneViewModel { get; set; }


        public MainViewModel() { }

        public MainViewModel(CustomModel model)
        {
            Debug1();
            Debug2();

            Model = model;
            _entityList = new CustomEntityList();            

            Model.InitializeScene += (o, e) =>
            {
                DrawingViewModel = new DrawingViewModel(Config.Drawing, Model);

                GeophonesViewModel = new GeophonesViewModel(
                    Model,
                    EntityList,
                    Config.Geophones);

                GeophoneViewModel = new GeophoneViewModel();

                Model.ZoomFit();
            };
        }

        private void Debug1()
        {
            Config = new Configuration
            {
                Drawing = new Drawing(@"C:\Users\Александер\Documents\GitHub\SensorSensitivity3D\SensorSensitivity3D\Pioner.dxf"),
            };
        }
        private void Debug2()
        {            
            Config.Geophones = new List<Geophone>();

            for (var i = 0; i < 20; ++i)
            {
                Config.Geophones.Add(new Geophone
                {
                    Name = $"Геофон {i}",
                    HoleNumber = i,
                    X = rnd.Next(-820, -450),
                    Y = rnd.Next(-1260, -990),
                    Z = rnd.Next(100, 210),
                    IsGood = true,
                    Color = $"#{Color.FromArgb(70, rnd.Next(256), rnd.Next(256), rnd.Next(256)).Name}",
                    GIsVisible = rnd.Next(0, 2) > 0,
                    SIsVisible = rnd.Next(0, 2) > 0,
                    R = 50
                   
                });

            }
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
