// <copyright file="StrategyPlugInProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System.Collections.Generic;
    using System.Reflection;
    using log4net;

    /// <summary>
    /// The implementation for the <see cref="IStrategyPlugInProvider{TKey,TPlugIn}"/> which provides plugins by their key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
    /// <seealso cref="MUnique.OpenMU.PlugIns.IStrategyPlugInProvider{TKey, TPlugIn}" />
    /// <seealso cref="IPlugInContainer{TPlugIn}" />
    public class StrategyPlugInProvider<TKey, TPlugIn> : PlugInContainerBase<TPlugIn>, IStrategyPlugInProvider<TKey, TPlugIn>
        where TPlugIn : class, IStrategyPlugIn<TKey>
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDictionary<TKey, TPlugIn> effectiveStrategies = new Dictionary<TKey, TPlugIn>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StrategyPlugInProvider{TKey, TPlugIn}"/> class.
        /// </summary>
        /// <param name="manager">The plugin manager which manages this instance.</param>
        public StrategyPlugInProvider(PlugInManager manager)
            : base(manager)
        {
        }

        /// <inheritdoc />
        public TPlugIn this[TKey key]
        {
            get
            {
                this.LockSlim.EnterReadLock();
                try
                {
                    if (this.TryGetPlugIn(key, out var plugIn))
                    {
                        return plugIn;
                    }
                }
                finally
                {
                    this.LockSlim.ExitReadLock();
                }

                return default;
            }
        }

        /// <summary>
        /// Tries the get the plug in with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="plugIn">The plug in.</param>
        /// <returns><c>True</c>, if the plugin has been found and returned; Otherwise, <c>false</c>.</returns>
        protected bool TryGetPlugIn(TKey key, out TPlugIn plugIn) => this.effectiveStrategies.TryGetValue(key, out plugIn);

        /// <inheritdoc />
        protected override void ActivatePlugIn(TPlugIn plugIn)
        {
            base.ActivatePlugIn(plugIn);
            if (this.effectiveStrategies.TryGetValue(plugIn.Key, out var registeredPlugIn))
            {
                this.log.Warn($"Plugin {registeredPlugIn} with key {plugIn.Key} was already registered and is active. Plugin {plugIn} will not be effective.");
            }
            else
            {
                this.SetEffectivePlugin(plugIn);
            }
        }

        /// <inheritdoc />
        protected override void DeactivatePlugIn(TPlugIn plugIn)
        {
            base.DeactivatePlugIn(plugIn);
            if (this.effectiveStrategies.TryGetValue(plugIn.Key, out var effective) && effective == plugIn)
            {
                this.effectiveStrategies.Remove(plugIn.Key);
            }
        }

        /// <summary>
        /// Sets the effective plugin.
        /// </summary>
        /// <param name="plugIn">The plug in.</param>
        protected void SetEffectivePlugin(TPlugIn plugIn)
        {
            this.effectiveStrategies.Remove(plugIn.Key);
            this.effectiveStrategies.Add(plugIn.Key, plugIn);
        }
    }
}
