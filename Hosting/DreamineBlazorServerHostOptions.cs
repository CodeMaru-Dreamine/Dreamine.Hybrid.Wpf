using System;
using System.Collections.Generic;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// \brief Represents options for hosting a Blazor Server endpoint inside a WPF process.
    /// </summary>
    public sealed class DreamineBlazorServerHostOptions
    {
        /// <summary>
        /// \brief Gets or sets the localhost port used by the embedded Blazor Server host.
        /// </summary>
        public int Port { get; set; } = 5000;

        /// <summary>
        /// \brief Gets or sets the content root path used by the embedded Blazor Server host.
        /// </summary>
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        /// <summary>
        /// \brief Gets or sets a value indicating whether public ViewModel classes should be automatically registered.
        /// </summary>
        public bool AutoRegisterViewModels { get; set; } = true;

        /// <summary>
        /// \brief Gets service types that should be shared from the WPF host service provider to the Blazor Server host.
        /// </summary>
        public IList<Type> SharedServiceTypes { get; } = new List<Type>();
    }
}