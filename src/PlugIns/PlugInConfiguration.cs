// <copyright file="PlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;

    /// <summary>
    /// Configuration for plugins.
    /// </summary>
    public class PlugInConfiguration
    {
        /// <summary>
        /// Gets or sets the type identifier of the plugin.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the custom plug in source which will be compiled at run-time.
        /// </summary>
        public string CustomPlugInSource { get; set; }

        /// <summary>
        /// Gets or sets the name of the external assembly which will be loaded at run-time.
        /// </summary>
        public string ExternalAssemblyName { get; set; }
    }
}