using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Domain.Models
{
    public class CustomModel : Model
    {
        public CustomModel()
        {
            Entities = new EntityList();
        }

        public static readonly DependencyProperty CustomEntityListProperty =
            DependencyProperty.Register(
                nameof(CustomEntityList),
                typeof(CustomEntityList),
                typeof(CustomModel),
                new FrameworkPropertyMetadata(default(CustomEntityList), OnCustomEntityListChanged));

        private static void OnCustomEntityListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vp = (CustomModel)d;
            var oldValue = (CustomEntityList)e.OldValue;
            if (oldValue != null)
                oldValue.Model = null;
            var newValue = (CustomEntityList)e.NewValue;
            if (newValue != null)
                newValue.Model = vp;
        }

        public CustomEntityList CustomEntityList
        {
            get => (CustomEntityList)GetValue(CustomEntityListProperty);
            set => SetValue(CustomEntityListProperty, value);
        }        
    }
}
