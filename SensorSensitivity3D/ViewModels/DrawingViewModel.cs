using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;

using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels
{

    public class DrawingViewModel : BaseViewModel, IDropTarget
    {
        private readonly ConfigService _configService;
        private readonly Configuration _config;
        
        public DrawingViewModel() { }

        public DrawingViewModel(ConfigService configService, Configuration config)
        {
            _configService = configService;
            _config = config;

            UpdateSubstrate(config.SubstratePath);
        }



        #region Commands
        
        private RelayCommand _loadModelCommand;
        public ICommand LoadModelCommand
            => _loadModelCommand ??= new RelayCommand(ExecuteLoadModelCommand);
        
        private void ExecuteLoadModelCommand(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = System.Environment.CurrentDirectory,
                Filter = "Drawing Format (*.dxf;*.dwg)|*.dxf;*dwg"
            };

            if (openFileDialog.ShowDialog() != true) return;

            if (UpdateSubstrate(openFileDialog.FileName))
                _configService.EditConfiguration(_config);
        }

        private RelayCommand _editModelCommand;
        public ICommand EditModelCommand
            => _editModelCommand ??= new RelayCommand(ExecuteEditModelCommand, CanExecuteEditModelCommand);

        private void ExecuteEditModelCommand(object obj)
        {

        }

        private bool CanExecuteEditModelCommand(object o)
            => true;
        
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
