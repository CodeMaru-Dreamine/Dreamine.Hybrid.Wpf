using System;

namespace Dreamine.Hybrid.Wpf.Interfaces
{
    /// <summary>
    /// \brief Provides a contract for application classes that need access to the root service provider.
    /// </summary>
    public interface IDreamineServiceProviderAware
    {
        /// <summary>
        /// \brief Sets the root service provider for the application.
        /// </summary>
        /// <param name="serviceProvider">The root service provider.</param>
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}