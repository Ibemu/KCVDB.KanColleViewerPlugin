using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KCVDB.KanColleViewerPlugin.Views.Converters
{
	class BoolConverterBase<TValue> : IValueConverter
	{
		public TValue TrueValue { get; set; }
		public TValue FalseValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class BoolBrushConverter : BoolConverterBase<Brush>
	{ }
}
