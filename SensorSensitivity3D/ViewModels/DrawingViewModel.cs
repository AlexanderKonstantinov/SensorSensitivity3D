using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using SensorSensitivity3D.ViewModels.Base;
using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels
{

    public class DrawingViewModel : BaseViewModel, IDropTarget
    {
        private readonly ConfigService _configService;
        private readonly Configuration _config;

        public Drawing _drawing;
        public Drawing Drawing
        {
            get => _drawing;
            set
            {
                _drawing = value;
                OnPropertyChanged(nameof(Drawing));
            }
        }
        public double XMin { get; set; }
        
        public DrawingViewModel() { }

        public DrawingViewModel(ConfigService configService, Configuration config)
        {
            _configService = configService;
            _config = config;

            UpdateSubstrate(config.SubstratePath, _config.DrawingIsVisible, ref _drawing);
            OnPropertyChanged(nameof(Drawing));
        }

        #region Commands
        
        private RelayCommand _loadDrawingCommand;
        public ICommand LoadDrawingCommand
            => _loadDrawingCommand ??= new RelayCommand(ExecuteLoadDrawingCommand);
        
        private void ExecuteLoadDrawingCommand(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = System.Environment.CurrentDirectory,
                Filter = "Drawing Format (*.dxf;*.dwg)|*.dxf;*dwg"
            };

            if (openFileDialog.ShowDialog() != true) return;



            if (UpdateSubstrate(openFileDialog.FileName, true, ref _drawing))
            {
                _config.DrawingIsVisible = true;
                _config.SubstratePath = openFileDialog.FileName;
                _configService.EditConfiguration(_config);                
                OnPropertyChanged(nameof(Drawing));
                ZoomFit();
            }
        }

        private RelayCommand _showDrawingCommand;
        public ICommand ShowDrawingCommand
            => _showDrawingCommand ??= new RelayCommand(ExecuteShowDrawingCommand, CanExecuteShowDrawingCommand);

        private void ExecuteShowDrawingCommand(object obj)
            => SwitchEntitiesVisibility(Drawing.Entities, Drawing.IsVisible);
        

        private bool CanExecuteShowDrawingCommand(object obj)
            => Drawing?.Entities.Length > 0;

            #endregion


        public void DragOver(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();

            if (dragFileList.Count() != 1) return;

            var extension = Path.GetExtension(dragFileList.First());

            dropInfo.Effects = extension != null && (extension.Equals(".dxf") || extension.Equals(".dwg"))
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
            => throw new NotImplementedException();

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
