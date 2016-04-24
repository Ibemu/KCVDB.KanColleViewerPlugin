using System;
using System.Globalization;
using System.Windows.Data;

namespace KCVDB.KanColleViewerPlugin.Views.Converters
{
	class SingleParameterResourceStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var resourceName = (string)parameter;
			var template = ResourceHolder.Instance.GetString(resourceName);
			return string.Format(template, value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class MultipleParameterResourceStringConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var resourceName = (string)parameter;
			var template = ResourceHolder.Instance.GetString(resourceName);
			return string.Format(template, values);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
