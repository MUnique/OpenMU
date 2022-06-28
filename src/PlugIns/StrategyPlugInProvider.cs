// <copyright file="StrategyPlugInProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

/// <summary>
/// The implementation for the <see cref="IStrategyPlugInProvider{TKey,TPlugIn}"/> which provides plugins by their key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
/// <seealso cref="MUnique.OpenMU.PlugIns.IStrategyPlugInProvider{TKey, TPlugIn}" />
/// <seealso cref="IPlugInContainer{TPlugIn}" />
public class StrategyPlugInProvider<TKey, TPlugIn> : PlugInContainerBase<TPlugIn>, IStrategyPlugInProvider<TKey, TPlugIn>
    where TPlugIn : class, IStrategyPlugIn<TKey>
    where TKey : notnull
{
    private readonly IDictionary<TKey, TPlugIn> _effectiveStrategies = new Dictionary<TKey, TPlugIn>();

    /// <summary>
    /// Initializes a new instance of the <see cref="StrategyPlugInProvider{TKey, TPlugIn}" /> class.
    /// </summary>
    /// <param name="manager">The plugin manager which manages this instance.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public StrategyPlugInProvider(PlugInManager manager, ILoggerFactory loggerFactory)
        : base(manager)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <value>
    /// The logger.
    /// </value>
    protected ILogger Logger { get; }

    /// <inheritdoc />
    public TPlugIn? this[TKey key]
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

    /// <inheritdoc />
    public IEnumerable<TPlugIn> AvailableStrategies
    {
        get
        {
            this.LockSlim.EnterReadLock();
            try
            {
                return this._effectiveStrategies.Values.ToList();
            }
            finally
            {
                this.LockSlim.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Tries the get the plug in with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="plugIn">The plug in.</param>
    /// <returns><c>True</c>, if the plugin has been found and returned; Otherwise, <c>false</c>.</returns>
    protected bool TryGetPlugIn(TKey key, [MaybeNullWhen(false)] out TPlugIn plugIn) => this._effectiveStrategies.TryGetValue(key, out plugIn);

    /// <inheritdoc />
    protected override void ActivatePlugIn(TPlugIn plugIn)
    {
        base.ActivatePlugIn(plugIn);
        if (this._effectiveStrategies.TryGetValue(plugIn.Key, out var registeredPlugIn))
        {
            this.Logger.LogWarning($"Plugin {registeredPlugIn} with key {plugIn.Key} was already registered and is active. Plugin {plugIn} will not be effective.");
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
        if (this._effectiveStrategies.TryGetValue(plugIn.Key, out var effective) && effective == plugIn)
        {
            this._effectiveStrategies.Remove(plugIn.Key);
        }
    }

    /// <summary>
    /// Sets the effective plugin.
    /// </summary>
    /// <param name="plugIn">The plug in.</param>
    protected void SetEffectivePlugin(TPlugIn plugIn)
    {
        this._effectiveStrategies.Remove(plugIn.Key);
        this._effectiveStrategies.Add(plugIn.Key, plugIn);
    }
}