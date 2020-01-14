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

        /// <summary>
        /// Навигация на выбранные объекты
        /// </summary>
        /// <param name="entities"></param>
        public void GoToEntities(IList<Entity> entities)
        {
            // Если объекты невидимые навигация не происходит
            var visibilitiesSafe = entities.Select(e => e.Visible).ToList();

            foreach (var e in entities) 
                e.Visible = true;

            this.ZoomFit(entities, false, 20);

            for (int i = 0; i < entities.Count; i++)
                entities[i].Visible = visibilitiesSafe[i];

            this.Invalidate();
        }
    }
}
