// <copyright file="CustomPlugInContainerAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;

    /// <summary>
    /// An attribute which describes a custom plugin container interface.
    /// May be helpful for debugging and the user interface.
    /// A custom plugin container is used to manage this plugin point.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class CustomPlugInContainerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPlugInContainerAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the plugin container.</param>
        /// <param name="description">The description of the plugin container.</param>
        public CustomPlugInContainerAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// Gets the name of the custom plugin container.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the custom plugin container.
        /// </summary>
        public string Description { get; }
    }
}