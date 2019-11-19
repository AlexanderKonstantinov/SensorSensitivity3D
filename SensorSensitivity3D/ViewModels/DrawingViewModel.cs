using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using devDept.Eyeshot;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Helpers;

namespace SensorSensitivity3D.ViewModels
{
    public class DrawingViewModel : BaseViewModel, IDropTarget
    {
        public event Action<string> DrawingLoaded; 
        
        public Drawing Drawing { get; set; }
        public readonly Model _model;
        
        public DrawingViewModel() { }

        public DrawingViewModel(Drawing drawing, Model model)
        {
            Drawing = drawing;
            _model = model;
        }


        #region Commands
        
        private RelayCommand _loadModelCommand;
        public ICommand LoadModelCommand
            => _loadModelCommand
               ?? (_loadModelCommand = new RelayCommand(ExecuteLoadModelCommand, CanExecuteLoadModelCommand));
        
        private void ExecuteLoadModelCommand(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = System.Environment.CurrentDirectory,
                Filter = "Drawing Format (*.dxf;*.dwg)|*.dxf;*dwg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DrawingLoaded?.Invoke(openFileDialog.FileName);
                UpdateDrawing(openFileDialog.FileName);
                OnPropertyChanged(nameof(Drawing));
            }
        }

        private void UpdateDrawing(string path)
        {
            Drawing.ModelPath = path;
            Drawing.XMin = _model.Entities.BoxMin.X;
            Drawing.XMax = _model.Entities.BoxMax.X;
            Drawing.YMin = _model.Entities.BoxMin.Y;
            Drawing.YMax = _model.Entities.BoxMax.Y;
            Drawing.ZMin = _model.Entities.BoxMin.Z;
            Drawing.ZMax = _model.Entities.BoxMax.Z;
        }

        private bool CanExecuteLoadModelCommand(object o)
            => true;

        private RelayCommand _editModelCommand;
        public ICommand EditModelCommand
            => _editModelCommand
               ?? (_editModelCommand = new RelayCommand(ExecuteEditModelCommand, CanExecuteEditModelCommand));

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
