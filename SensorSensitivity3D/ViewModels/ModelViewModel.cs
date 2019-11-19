using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using devDept.Eyeshot;
using devDept.Eyeshot.Translators;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.ViewModels
{
    public class ModelViewModel
    {
        public readonly CustomModel _model;
        private readonly Drawing _drawing;

        public ModelViewModel(CustomModel model, Drawing drawing)
        {
            _drawing = drawing;
            _model = model;


            _model.InitializeScene += (o, e) =>
            {
                Redraw(_drawing.ModelPath);
            };
        }

        public void Redraw(string path)
        {
            if ((_drawing?.ModelPath == path && _model.Entities.Count > 0) ||
                !File.Exists(path))
                return;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));

            var rfa = new ReadAutodesk(path);
            rfa.DoWork();

            _model.Clear();

            rfa.AddToScene(_model);

            //Entity[] toAdd = Model.Entities.Explode();
            //Model.Entities.AddRange(toAdd);
            //Model.SetView(viewType.Top);

            _model.ZoomFit();
            _model.Invalidate();

            
        }
    }
}
