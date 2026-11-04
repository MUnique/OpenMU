// <copyright file="FeaturePlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin container for <see cref="IFeaturePlugIn"/>s.
/// </summary>
public class FeaturePlugInContainer : PlugInContainerBase<IFeaturePlugIn>, ICustomPlugInContainer<IFeaturePlugIn>
{
    private readonly ConcurrentDictionary<Type, IFeaturePlugIn> _currentlyEffectivePlugIns = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FeaturePlugInContainer"/> class.
    /// </summary>
    /// <param name="manager">The plugin manager which manages this instance.</param>
    public FeaturePlugInContainer(PlugInManager manager)
        : base(manager)
    {
        foreach (var plugIn in this.Manager.GetActivePlugInsOf<IFeaturePlugIn>())
        {
            if (!this._currentlyEffectivePlugIns.ContainsKey(plugIn.GetType()))
            {
                this.AddPlugIn(plugIn, true);
            }
        }
    }

    /// <inheritdoc />
    public T? GetPlugIn<T>()
        where T : class, IFeaturePlugIn
    {
        if (this._currentlyEffectivePlugIns.TryGetValue(typeof(T), out var plugIn) && plugIn is T t)
        {
            return t;
        }

        return default;
    }

    /// <inheritdoc />
    protected override void ActivatePlugIn(IFeaturePlugIn plugIn)
    {
        var plugInType = plugIn.GetType();
        if (this._currentlyEffectivePlugIns.ContainsKey(plugInType))
        {
            return;
        }

        base.ActivatePlugIn(plugIn);
        this._currentlyEffectivePlugIns.TryAdd(plugInType, plugIn);
    }

    /// <inheritdoc />
    protected override void DeactivatePlugIn(IFeaturePlugIn plugIn)
    {
        base.DeactivatePlugIn(plugIn);
        this._currentlyEffectivePlugIns.TryRemove(plugIn.GetType(), out _);
    }

    /// <inheritdoc/>
    protected override void BeforeActivatePlugInType(Type plugInType)
    {
        base.BeforeActivatePlugInType(plugInType);

        var knownPlugIn = this.FindKnownPlugin(plugInType);
        if (knownPlugIn is null)
        {
            this.AddPlugIn((IFeaturePlugIn)Activator.CreateInstance(plugInType)!, true);
        }
    }
}