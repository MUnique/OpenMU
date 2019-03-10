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
        private readonly IDictionary<TKey, TPlugIn> activeStrategies = new Dictionary<TKey, TPlugIn>();

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
                    if (this.activeStrategies.TryGetValue(key, out var plugIn))
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

        /// <inheritdoc />
        protected override void ActivatePlugIn(TPlugIn plugIn)
        {
            base.ActivatePlugIn(plugIn);
            if (this.activeStrategies.TryGetValue(plugIn.Key, out var registeredPlugIn))
            {
                this.log.Warn($"Plugin {registeredPlugIn} with key {plugIn.Key} was already registered and is active.");
            }
            else
            {
                this.activeStrategies.Add(plugIn.Key, plugIn);
            }
        }

        /// <inheritdoc />
        protected override void DeactivatePlugIn(TPlugIn plugIn)
        {
            base.DeactivatePlugIn(plugIn);
            this.activeStrategies.Remove(plugIn.Key);
        }
    }
}
