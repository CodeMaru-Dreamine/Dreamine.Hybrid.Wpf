using System;
using System.ComponentModel;
using System.Windows;

namespace Dreamine.Hybrid.Wpf.Utility
{
    /// <summary>
    /// Provides cached WPF design-time detection.
    /// </summary>
    public static class DesignTimeGuard
    {
        private static readonly bool _isInDesignMode = DetectDesignMode();

        /// <summary>
        /// Gets a value indicating whether the current process is running in the WPF designer.
        /// </summary>
        public static bool IsInDesignMode => _isInDesignMode;

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
