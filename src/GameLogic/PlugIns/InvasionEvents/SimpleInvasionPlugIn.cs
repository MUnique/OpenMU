// <copyright file="SimpleInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// Convenience base for invasion plugins that need no logic beyond choosing a display
/// map and returning a default configuration.
/// </summary>
public abstract class SimpleInvasionPlugIn
    : BaseInvasionPlugIn<PeriodicInvasionConfiguration>, ISupportDefaultCustomConfiguration
{
    private readonly IReadOnlyList<ushort> _displayMaps;
    private readonly Func<PeriodicInvasionConfiguration> _defaultConfigFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleInvasionPlugIn"/> class.
    /// </summary>
    /// <param name="mapEventType">The map-event type for UI broadcasting.</param>
    /// <param name="displayMaps">Maps eligible to be shown in the event UI.</param>
    /// <param name="defaultConfigFactory">Factory that returns the default configuration.</param>
    protected SimpleInvasionPlugIn(MapEventType mapEventType, IReadOnlyList<ushort> displayMaps, Func<PeriodicInvasionConfiguration> defaultConfigFactory)
        : base(mapEventType)
    {
        this._displayMaps = displayMaps;
        this._defaultConfigFactory = defaultConfigFactory;
    }

    /// <inheritdoc />
    protected override IReadOnlyList<ushort> EventDisplayMapIds => this._displayMaps;

    /// <inheritdoc />
    public object CreateDefaultConfig() => this._defaultConfigFactory();
}