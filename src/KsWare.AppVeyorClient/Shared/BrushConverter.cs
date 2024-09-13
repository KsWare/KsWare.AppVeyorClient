using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace KsWare.AppVeyorClient.Shared;

public class BrushConverterEx : IValueConverter {

	private static readonly System.Windows.Media.ColorConverter CC = new();

	public static readonly BrushConverterEx Default = new();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}

	public Brush ConvertFrom(object value) => new SolidColorBrush((Color) CC.ConvertFrom(value));
}
