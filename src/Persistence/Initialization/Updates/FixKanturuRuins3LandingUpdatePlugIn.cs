// <copyright file="FixKanturuRuins3LandingUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Narrows the seeded Kanturu Ruins 3 landing area to walkable terrain.
/// </summary>
/// <remarks>
/// The old random-placement rectangle includes 19 blocked tiles. Only its exact seeded bounds
/// and references belonging to this configuration are repaired; customized destinations are kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.FixKanturuRuins3LandingUpdatePlugIn_Name), Description = nameof(Properties.PlugInResources.FixKanturuRuins3LandingUpdatePlugIn_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("51ED8837-02FA-4F32-89F5-2837951ED46A")]
public class FixKanturuRuins3LandingUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The Kanturu Ruins 3 entry in the Season 6 client's warp list.
    /// </summary>
    private const int KanturuRuins3WarpIndex = 45;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.FixKanturuRuins3LandingUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.FixKanturuRuins3LandingUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 04, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var warp in gameConfiguration.WarpList.Where(warp => warp.Index == KanturuRuins3WarpIndex))
        {
            if (warp.Gate is { X1: 66, Y1: 183, X2: 74, Y2: 191, Map: { Number: KanturuRuins.Number, Discriminator: 0 } map } gate
                && gameConfiguration.Maps.Any(owned => ReferenceEquals(owned, map))
                && map.ExitGates.Any(owned => ReferenceEquals(owned, gate)))
            {
                gate.X1 = 67;
                gate.Y2 = 189;
            }
        }

        return ValueTask.CompletedTask;
    }
}
