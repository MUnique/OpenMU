// <copyright file="CustomPlugInContainerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A base class for custom plugin containers, where <typeparamref name="TPlugIn"/> defines the common interface type of the plugin container.
    /// The custom plugin container basically collects all plugins of this type, and provides the method <see cref="GetPlugIn{T}"/> to retrieve
    /// a specific plugin interface. For a specific plugin interface, there can only be one 'effective' implementation.
    /// This base class defines abstract methods which help to select these 'effective' implementations.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
    /// <seealso cref="MUnique.OpenMU.PlugIns.PlugInContainerBase{TPlugIn}" />
    public abstract class CustomPlugInContainerBase<TPlugIn> : PlugInContainerBase<TPlugIn>, ICustomPlugInContainer<TPlugIn>
        where TPlugIn : class
    {
        private readonly ConcurrentDictionary<Type, TPlugIn> currentlyEffectivePlugIns = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPlugInContainerBase{TPlugIn}"/> class.
        /// </summary>
        /// <param name="manager">The plugin manager which manages this instance.</param>
        protected CustomPlugInContainerBase(PlugInManager manager)
            : base(manager)
        {
            if (typeof(TPlugIn).GetCustomAttribute<CustomPlugInContainerAttribute>() is null)
            {
                throw new ArgumentException($"The specified type argument {typeof(TPlugIn)} isn't marked with the {typeof(CustomPlugInContainerAttribute)}.");
            }
        }

        /// <inheritdoc />
        public T? GetPlugIn<T>()
            where T : class, TPlugIn
        {
            if (this.currentlyEffectivePlugIns.TryGetValue(typeof(T), out var plugIn) && plugIn is T t)
            {
                return t;
            }

            return default;
        }

        /// <summary>
        /// Initializes the active plugins of <typeparamref name="TPlugIn"/> in this instance.
        /// This method should be called in the constructor of derived classes.
        /// </summary>
        protected void Initialize()
        {
            foreach (var plugInType in this.Manager.GetKnownPlugInsOf<TPlugIn>().Where(this.Manager.IsPlugInActive))
            {
                if (!this.currentlyEffectivePlugIns.ContainsKey(plugInType))
                {
                    this.CreatePlugInIfSuitable(plugInType);
                }
            }
        }

        /// <inheritdoc />
        protected override void ActivatePlugIn(TPlugIn plugIn)
        {
            base.ActivatePlugIn(plugIn);
            foreach (var viewInterface in GetInterfaceTypes(plugIn))
            {
                if (this.currentlyEffectivePlugIns.TryGetValue(viewInterface, out var currentEffectivePlugIn))
                {
                    if (this.IsNewPlugInReplacingOld(currentEffectivePlugIn, plugIn))
                    {
                        this.currentlyEffectivePlugIns.TryUpdate(viewInterface, plugIn, currentEffectivePlugIn);
                    }
                }
                else
                {
                    this.currentlyEffectivePlugIns.TryAdd(viewInterface, plugIn);
                }
            }
        }

        /// <inheritdoc />
        protected override void DeactivatePlugIn(TPlugIn plugIn)
        {
            base.DeactivatePlugIn(plugIn);
            foreach (var viewInterface in GetInterfaceTypes(plugIn))
            {
                if (this.currentlyEffectivePlugIns.TryGetValue(viewInterface, out var currentEffectivePlugIn)
                    && currentEffectivePlugIn == plugIn
                    && this.currentlyEffectivePlugIns.TryRemove(viewInterface, out _))
                {
                    if (this.DetermineEffectivePlugIn(viewInterface) is { } newEffectivePlugIn)
                    {
                        this.currentlyEffectivePlugIns.TryAdd(viewInterface, newEffectivePlugIn);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the new plug in should replace the currently effective plug in.
        /// </summary>
        /// <param name="currentEffectivePlugIn">The current effective plug in.</param>
        /// <param name="activatedPlugIn">The activated plug in.</param>
        /// <returns>
        ///   <c>true</c> if the new plug in should replace the currently effective plug in; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsNewPlugInReplacingOld(TPlugIn currentEffectivePlugIn, TPlugIn activatedPlugIn);

        /// <summary>
        /// Determines the new effective plug in, after the previous one has been deactivated.
        /// </summary>
        /// <param name="interfaceType">The interface type of the actual plugin type.</param>
        /// <returns>The new effective plugin.</returns>
        protected abstract TPlugIn? DetermineEffectivePlugIn(Type interfaceType);

        /// <summary>
        /// Creates the plug in if suitable for this instance.
        /// </summary>
        /// <param name="plugInType">Type of the plug in.</param>
        protected abstract void CreatePlugInIfSuitable(Type plugInType);

        /// <inheritdoc/>
        protected override void BeforeActivatePlugInType(Type plugInType)
        {
            base.BeforeActivatePlugInType(plugInType);

            var knownPlugIn = this.FindKnownPlugin(plugInType);
            if (knownPlugIn is null)
            {
                this.CreatePlugInIfSuitable(plugInType);
            }
        }

        private static IEnumerable<Type> GetInterfaceTypes(TPlugIn plugIn) => plugIn.GetType().GetInterfaces().Where(i => i.GetInterfaces().Contains(typeof(TPlugIn)));
    }
}