// <copyright file="PlugInConfigurationViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Models
{
    using System;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Data transfer object for <see cref="PlugInConfiguration"/>.
    /// </summary>
    public class PlugInConfigurationViewItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInConfigurationViewItem"/> class.
        /// </summary>
        /// <param name="plugInConfiguration">The plug in configuration.</param>
        public PlugInConfigurationViewItem(PlugInConfiguration plugInConfiguration)
        {
            this.Configuration = plugInConfiguration;
        }

        /// <summary>
        /// Gets the underlying configuration object.
        /// </summary>
        public PlugInConfiguration Configuration { get; }

        /// <summary>
        /// Gets or sets the identifier of the plugin configuration.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the game configuration identifier to which the plugin configuration belongs to.
        /// </summary>
        public Guid GameConfigurationId { get; set; }

        /// <summary>
        /// Gets or sets the type identifier of the plugin.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the plugin type.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the plugin.
        /// </summary>
        public string PlugInName { get; set; }

        /// <summary>
        /// Gets or sets the plugin description.
        /// </summary>
        public string PlugInDescription { get; set; }

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

        /// <summary>
        /// Gets or sets the name of the plug in point.
        /// </summary>
        public string PlugInPointName { get; set; }

        /// <summary>
        /// Gets or sets the plug in point description.
        /// </summary>
        public string PlugInPointDescription { get; set; }
    }
}