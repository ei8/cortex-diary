/*
    This file is part of the d# project.
    Copyright (c) 2016-2020 ei8
    Authors: ei8
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    EI8. EI8 DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS
     This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    https://github.com/ei8/cortex-diary/blob/master/LICENSE
     The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
     You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the d# software without
    disclosing the source code of your own applications.
     For more information, please contact ei8 at this address: 
     support@ei8.works
 */

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    // https://stackoverflow.com/a/32526689
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    class BoolToColorBrushConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Bolean value controlling wether to apply color change</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">A CSV string on the format [ColorNameIfTrue;ColorNameIfFalse;OpacityNumber] may be provided for customization, default is [LimeGreen;Transperent;1.0].</param>
        /// <param name="culture"></param>
        /// <returns>A SolidColorBrush in the supplied or default colors depending on the state of value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush color;
            // Setting default values
            var colorIfTrue = Colors.LimeGreen;
            var colorIfFalse = Colors.Transparent;
            double opacity = 1;
            // Parsing converter parameter
            if (parameter != null)
            {
                // Parameter format: [ColorNameIfTrue;ColorNameIfFalse;OpacityNumber]
                var parameterstring = parameter.ToString();
                if (!string.IsNullOrEmpty(parameterstring))
                {
                    var parameters = parameterstring.Split(';');
                    var count = parameters.Length;
                    if (count > 0 && !string.IsNullOrEmpty(parameters[0]))
                    {
                        colorIfTrue = ColorFromName(parameters[0]);
                    }
                    if (count > 1 && !string.IsNullOrEmpty(parameters[1]))
                    {
                        colorIfFalse = ColorFromName(parameters[1]);
                    }
                    if (count > 2 && !string.IsNullOrEmpty(parameters[2]))
                    {
                        double dblTemp;
                        if (double.TryParse(parameters[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out dblTemp))
                            opacity = dblTemp;
                    }
                }
            }
            // Creating Color Brush
            if ((bool)value)
            {
                color = new SolidColorBrush(colorIfTrue);
                color.Opacity = opacity;
            }
            else
            {
                color = new SolidColorBrush(colorIfFalse);
                color.Opacity = opacity;
            }
            return color;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static Color ColorFromName(string colorName)
        {
            System.Drawing.Color systemColor = System.Drawing.Color.FromName(colorName);
            return Color.FromArgb(systemColor.A, systemColor.R, systemColor.G, systemColor.B);
        }
    }
}
