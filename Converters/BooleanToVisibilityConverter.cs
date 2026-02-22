/// \file BooleanToVisibilityConverter.cs
/// \brief bool ↔ Visibility 변환기 (디자인 타임 지원)
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dreamine.Hybrid.Wpf.Converters
{
	/// <summary>Boolean을 Visibility로 변환하는 변환기</summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BooleanToVisibilityConverter : IValueConverter
	{
		/// <summary>싱글톤 인스턴스</summary>
		public static BooleanToVisibilityConverter Instance { get; } = new();

		/// <inheritdoc/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b)
				return b ? Visibility.Visible : Visibility.Collapsed;
			return Visibility.Collapsed;
		}

		/// <inheritdoc/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility v)
				return v == Visibility.Visible;
			return false;
		}
	}
}
