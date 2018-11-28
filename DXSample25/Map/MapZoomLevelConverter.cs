using System;
using System.Globalization;
using System.Windows.Data;
using DevExpress.Xpf.Map;

namespace DXSample25.Map
{
    public class MapZoomLevelConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value is Double d && d > 0) return d;
            return MapControl.ZoomLevelProperty.DefaultMetadata.DefaultValue;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value is Double d && d > 0) return d;
            return MapControl.ZoomLevelProperty.DefaultMetadata.DefaultValue;
        }
    }
}