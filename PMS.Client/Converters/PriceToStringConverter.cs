using System.Globalization;

namespace PMS.Client.Converters;

public sealed class PriceToStringConverter : IValueConverter
{
	object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not int price)
			return "";

		var pounds = price / 100;
		var pennies = price % 100;

		return $"{pounds}.{pennies:00}";
	}

	object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		// TODO: Implement if we need to convert inputted prices by a user
		throw new NotImplementedException();
	}
}
