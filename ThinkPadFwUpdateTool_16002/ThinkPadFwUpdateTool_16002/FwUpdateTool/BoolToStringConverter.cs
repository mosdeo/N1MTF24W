using System;
using System.Globalization;
using System.Windows.Data;

namespace FwUpdateTool
{
	public class BoolToStringConverter : IValueConverter
	{
		public string FalseValue
		{
			get;
			set;
		}

		public string TrueValue
		{
			get;
			set;
		}

		public string NullValue
		{
			get;
			set;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return NullValue ?? FalseValue;
			}
			if (!(bool)value)
			{
				return FalseValue;
			}
			return TrueValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
