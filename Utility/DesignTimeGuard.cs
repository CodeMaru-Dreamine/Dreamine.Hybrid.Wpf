using System;
using System.ComponentModel;
using System.Windows;

namespace Dreamine.Hybrid.Wpf.Utility
{
    /// <summary>
    /// \if KO
    /// <para>캐시된 WPF 디자인 타임 감지 기능을 제공합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides cached WPF design-time detection.</para>
    /// \endif
    /// </summary>
    public static class DesignTimeGuard
    {
        /// <summary>
        /// \if KO
        /// <para>is In Design Mode 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the is in design mode value.</para>
        /// \endif
        /// </summary>
        private static readonly bool _isInDesignMode = DetectDesignMode();

        /// <summary>
        /// \if KO
        /// <para>현재 프로세스가 WPF 디자이너에서 실행 중인지 여부를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets whether the current process is running in the WPF designer.</para>
        /// \endif
        /// </summary>
        public static bool IsInDesignMode => _isInDesignMode;

        /// <summary>
        /// \if KO
        /// <para>Dispatcher 스레드 안전성을 지키면서 WPF 디자인 모드를 감지합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Detects WPF design mode while respecting dispatcher thread affinity.</para>
        /// \endif
        /// </summary>
        /// <returns>
        /// \if KO
        /// <para>디자인 모드이면 <see langword="true"/>, 감지할 수 없거나 런타임이면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para><see langword="true"/> in design mode; <see langword="false"/> at runtime or when detection is unavailable.</para>
        /// \endif
        /// </returns>
        private static bool DetectDesignMode()
        {
            try
            {
                if (Application.Current?.Dispatcher is { } dispatcher &&
                    !dispatcher.CheckAccess())
                {
                    return false;
                }

                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
