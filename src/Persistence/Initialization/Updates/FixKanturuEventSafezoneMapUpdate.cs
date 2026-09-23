// <copyright file="FixKanturuEventSafezoneMapUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Sets the safezone of the Kanturu event map to Kanturu Relics on databases which were
/// initialized before the event existed. Without it, players who die inside the event
/// respawn on the event map itself instead of Kanturu Relics.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("B906D994-0471-4AC5-9001-84AEADBFAC6A")]
public class FixKanturuEventSafezoneMapUpdate : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Kanturu Event Safezone Map";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Sets the safezone of the Kanturu event map to Kanturu Relics, so that players who die inside the event respawn there.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixKanturuEventSafezoneMap;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 22, 15, 32, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Maps.FirstOrDefault(m => m.Number == KanturuEvent.Number) is { } eventMap
            && eventMap.SafezoneMap is not { Number: KanturuRelics.Number })
        {
            eventMap.SafezoneMap = gameConfiguration.Maps.FirstOrDefault(m => m.Number == KanturuRelics.Number);
        }

        return ValueTask.CompletedTask;
    }
}
