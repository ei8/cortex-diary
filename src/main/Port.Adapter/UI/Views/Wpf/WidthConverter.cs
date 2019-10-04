using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class WidthConverter : MarkupExtension, IValueConverter
    {
            private static WidthConverter _instance;

            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return System.Convert.ToInt32(value) - System.Convert.ToInt32(parameter);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion

            public override object ProvideValue(IServiceProvider serviceProvider)
            {
                return _instance ?? (_instance = new WidthConverter());
            }
    }
}
