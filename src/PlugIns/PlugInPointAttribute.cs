// <copyright file="PlugInPointAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;

    /// <summary>
    /// An attribute which describes a plugin interface and its point.
    /// May be helpful for debugging and the user interface.
    /// A proxy class is automatically generated which executes all plugins which implement the marked interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class PlugInPointAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInPointAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the plugin point.</param>
        /// <param name="description">The description of the plugin point.</param>
        public PlugInPointAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// Gets the name of the plugin point.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the plugin point.
        /// </summary>
        public string Description { get; }
    }
}