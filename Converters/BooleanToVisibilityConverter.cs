// \file BooleanToVisibilityConverter.cs
// bool ↔ Visibility 변환기 (디자인 타임 지원)
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dreamine.Hybrid.Wpf.Converters
{
	/// <summary>
	/// \if KO
	/// <para>부울 값과 WPF <see cref="Visibility"/> 값을 상호 변환합니다.</para>
	/// \endif
	/// \if EN
	/// <para>Converts between Boolean values and WPF <see cref="Visibility"/> values.</para>
	/// \endif
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BooleanToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// \if KO
		/// <para>공유 싱글턴 인스턴스를 가져옵니다.</para>
		/// \endif
		/// \if EN
		/// <para>Gets the shared singleton instance.</para>
		/// \endif
		/// </summary>
		public static BooleanToVisibilityConverter Instance { get; } = new();

		/// <summary>
		/// \if KO
		/// <para><see langword="true"/>를 Visible로, 나머지를 Collapsed로 변환합니다.</para>
		/// \endif
		/// \if EN
		/// <para>Converts <see langword="true"/> to Visible and all other values to Collapsed.</para>
		/// \endif
		/// </summary>
		/// <param name="value">
		/// \if KO
		/// <para>변환할 값입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The value to convert.</para>
		/// \endif
		/// </param>
		/// <param name="targetType">
		/// \if KO
		/// <para>바인딩 대상 형식입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The binding target type.</para>
		/// \endif
		/// </param>
		/// <param name="parameter">
		/// \if KO
		/// <para>선택적 변환기 매개 변수입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The optional converter parameter.</para>
		/// \endif
		/// </param>
		/// <param name="culture">
		/// \if KO
		/// <para>변환 문화권입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The conversion culture.</para>
		/// \endif
		/// </param>
		/// <returns>
		/// \if KO
		/// <para>대응하는 가시성 값입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The corresponding visibility value.</para>
		/// \endif
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b)
				return b ? Visibility.Visible : Visibility.Collapsed;
			return Visibility.Collapsed;
		}

		/// <summary>
		/// \if KO
		/// <para>Visible을 <see langword="true"/>로, 나머지를 <see langword="false"/>로 변환합니다.</para>
		/// \endif
		/// \if EN
		/// <para>Converts Visible to <see langword="true"/> and all other values to <see langword="false"/>.</para>
		/// \endif
		/// </summary>
		/// <param name="value">
		/// \if KO
		/// <para>역변환할 값입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The value to convert back.</para>
		/// \endif
		/// </param>
		/// <param name="targetType">
		/// \if KO
		/// <para>바인딩 대상 형식입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The binding target type.</para>
		/// \endif
		/// </param>
		/// <param name="parameter">
		/// \if KO
		/// <para>선택적 변환기 매개 변수입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The optional converter parameter.</para>
		/// \endif
		/// </param>
		/// <param name="culture">
		/// \if KO
		/// <para>변환 문화권입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The conversion culture.</para>
		/// \endif
		/// </param>
		/// <returns>
		/// \if KO
		/// <para>가시성에 대응하는 부울 값입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The Boolean value corresponding to the visibility.</para>
		/// \endif
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility v)
				return v == Visibility.Visible;
			return false;
		}
	}
}
