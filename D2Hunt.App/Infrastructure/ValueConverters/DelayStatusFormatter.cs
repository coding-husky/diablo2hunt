using System.Globalization;
using System.Windows.Data;

namespace D2Hunt.App.Infrastructure.ValueConverters;

public class DelayStatusFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        int.TryParse(value?.ToString(), out var delay) && delay == 0 ? 0 : 1;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
