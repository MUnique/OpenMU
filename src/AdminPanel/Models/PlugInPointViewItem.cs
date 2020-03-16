// <copyright file="PlugInPointViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Models
{
    using System;

    /// <summary>
    /// View item for plugin extension points.
    /// </summary>
    public class PlugInPointViewItem
    {
        /// <summary>
        /// Gets or sets the type identifier of the plugin point.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the plug in point.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the plug in count.
        /// </summary>
        public int PlugInCount { get; set; }
    }
}