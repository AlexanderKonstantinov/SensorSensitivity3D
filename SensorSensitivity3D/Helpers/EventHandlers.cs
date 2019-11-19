using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using devDept.Eyeshot;

namespace SensorSensitivity3D.Helpers
{
    public static class EventHandlers
    {
        public static void RemoveClickEvent(ToolBarButton b)
        {
            FieldInfo f1 = typeof(ToolBarButton).GetField("Click",
                BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(b);
            PropertyInfo pi = b.GetType().GetProperty("Events",
                BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
            list.RemoveHandler(obj, list[obj]);
        }
    }
}
