using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class DynamicWidthConverter : MarkupExtension, IMultiValueConverter
    {
            private static DynamicWidthConverter _instance;

            #region IValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;

            if (double.Parse(values[0].ToString()) > int.Parse(parameter.ToString()))
            {
                result = System.Convert.ToDouble(values[1]) - System.Convert.ToDouble(values[2]) - System.Convert.ToDouble(values[3]);
                result = result < 0 ? 0 : result;
            }
            else
                result = System.Convert.ToDouble(values[1]);

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
            {
                return _instance ?? (_instance = new DynamicWidthConverter());
            }
    }
}
