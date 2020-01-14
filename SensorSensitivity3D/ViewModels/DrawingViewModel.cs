using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;

namespace SensorSensitivity3D.ViewModels
{

    public class DrawingViewModel : BaseViewModel, IDropTarget
    {
        private readonly DrawingService DrawingService;

        private readonly Model Model;
        private ReadAutodesk _readAutodesk;

        public Entity[] DrawingEntities => _readAutodesk?.Entities;

        public Drawing Drawing { get; set; }
        


        public DrawingViewModel() { }

        public DrawingViewModel(Model model, Configuration config)
        {
            Model = model;

            DrawingService = new DrawingService();
            Drawing = DrawingService.GetDrawing(config.Id);
            
            //TODO Проверка наличия файла 
            ReadAutodesk(Drawing.Path);
        }



        #region Commands
        
        private RelayCommand _loadModelCommand;
        public ICommand LoadModelCommand
            => _loadModelCommand ??= new RelayCommand(ExecuteLoadModelCommand, CanExecuteLoadModelCommand);
        
        private void ExecuteLoadModelCommand(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = System.Environment.CurrentDirectory,
                Filter = "Drawing Format (*.dxf;*.dwg)|*.dxf;*dwg"
            };

            if (openFileDialog.ShowDialog() == true)
                ReadAutodesk(openFileDialog.FileName);
        }

        private void ReadAutodesk(string drawingPath)
        {
            if (!File.Exists(drawingPath))
                return;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));

            _readAutodesk = new ReadAutodesk(drawingPath);
            _readAutodesk.DoWork();

            UpdateDrawing();
            _readAutodesk.AddToScene(Model);

            foreach (var e in Model.Entities)
                e.Selectable = false;
        }

        public void UpdateDrawing()
        {
            Drawing.Path = _readAutodesk.Path;
            Drawing.Name = new FileInfo(_readAutodesk.Path).Name;
            Drawing.XMin = _readAutodesk.Min.X;
            Drawing.XMax = _readAutodesk.Max.X;
            Drawing.YMin = _readAutodesk.Min.Y;
            Drawing.YMax = _readAutodesk.Max.Y;
            Drawing.ZMin = _readAutodesk.Min.Z;
            Drawing.ZMax = _readAutodesk.Max.Z;

            OnPropertyChanged(nameof(Drawing));
        }

        private bool CanExecuteLoadModelCommand(object o)
            => true;

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
