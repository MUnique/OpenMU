// <copyright file="PlugInAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;

    /// <summary>
    /// An attribute which describes an implementation of a plugin interface.
    /// May be helpful for debugging and the user interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PlugInAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public PlugInAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the plugin.
        /// </summary>
        public string Description { get; }
    }
}