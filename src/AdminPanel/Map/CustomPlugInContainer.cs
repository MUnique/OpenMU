// <copyright file="CustomPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin container which allows to manually register plugins.
    /// </summary>
    /// <typeparam name="T">The type of plugins.</typeparam>
    public class CustomPlugInContainer<T> : ICustomPlugInContainer<T>
    {
        private readonly IDictionary<Type, T> registeredPlugIns = new Dictionary<Type, T>();

        /// <summary>
        /// Registers the plug in.
        /// </summary>
        /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
        /// <param name="plugIn">The plug in.</param>
        public void RegisterPlugIn<TPlugIn>(TPlugIn plugIn)
            where TPlugIn : class, T
        {
            this.registeredPlugIns.Add(typeof(TPlugIn), plugIn);
        }

        /// <inheritdoc />
        public TPlugIn GetPlugIn<TPlugIn>()
            where TPlugIn : class, T
        {
            if (this.registeredPlugIns.TryGetValue(typeof(TPlugIn), out var plugIn)
                && plugIn is TPlugIn requestedPlugIn)
            {
                return requestedPlugIn;
            }

            return default;
        }
    }
}