using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SensorSensitivity3D.Helpers
{
    public class ObjectProperty
    {
        private object _source;

        private PropertyInfo _propertyInfo;
        public string PropertyName { get; }

        public object Value
        {
            get => _propertyInfo.GetValue(_source);
            set => _propertyInfo.SetValue(_source, Convert.ChangeType(value, _propertyInfo.PropertyType));
        }

        public ObjectProperty(object source, PropertyInfo propertyInfo)
        {
            _source = source;
            _propertyInfo = propertyInfo;
            var attributes = _propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);

            PropertyName = !attributes.Any() 
                ? _propertyInfo.Name 
                : attributes.Cast<DisplayNameAttribute>().Single().DisplayName;
        }
    }
}
