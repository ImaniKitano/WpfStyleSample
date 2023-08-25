using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace ImageViewer
{
    //! 論理値trueをVisibility.Visibleに、falseをVisibility.Corrapsedに変換するコンバータクラス。
    public class FalseCollapseConverter : IValueConverter
    {
        //! 論理値trueをVisibility.Visibleに、falseをVisibility.Collapsedに変換する。
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => ((bool)value) ? Visibility.Visible : Visibility.Collapsed;

        //! 逆の変換をサポートしない。
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }

    //! 論理値trueをVisibility.Collapsed、falseをVisibility.Visibleに変換するクラス。
    public class TrueCollapseConverter : IValueConverter
    {
        //! 論理値trueをVisibility.Collapsed、falseをVisibility.Visibleに変換する。
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => ((bool)value) ? Visibility.Collapsed : Visibility.Visible;

        //! 逆の変換をサポートしない。
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}
