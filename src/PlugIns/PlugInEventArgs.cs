// <copyright file="PlugInEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> regarding a plugin event.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class PlugInEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInEventArgs"/> class.
        /// </summary>
        /// <param name="plugInType">Type of the plug in.</param>
        public PlugInEventArgs(Type plugInType)
        {
            this.PlugInType = plugInType;
        }

        /// <summary>
        /// Gets the type of the plug in.
        /// </summary>
        /// <value>
        /// The type of the plug in.
        /// </value>
        public Type PlugInType { get; }
    }
}