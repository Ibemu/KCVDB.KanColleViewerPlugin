using System;
using System.Globalization;
using System.Windows.Data;

namespace KCVDB.KanColleViewerPlugin.Views.Converters
{
	class DateTimeOffsetToLongTimeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTimeOffset)value).LocalDateTime.ToLongTimeString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
