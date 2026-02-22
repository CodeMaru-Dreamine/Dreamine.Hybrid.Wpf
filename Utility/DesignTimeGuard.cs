/// \file DesignTimeGuard.cs
/// \brief 디자인 타임(Visual Studio 디자이너) 감지 유틸리티.
/// \date 2025-11-02

using System.ComponentModel;
using System.Windows;

namespace Dreamine.Hybrid.Wpf.Utility
{
	/// <summary>
	/// 디자인 타임 감지를 위한 정적 클래스.
	/// XAML에서 <c>{x:Static local:DesignTimeGuard.IsInDesignMode}</c> 형태로 사용 가능.
	/// </summary>
	public static class DesignTimeGuard
	{
		/// <summary>현재 디자이너 모드 여부</summary>
		public static bool IsInDesignMode =>
			DesignerProperties.GetIsInDesignMode(new DependencyObject());
	}
}
