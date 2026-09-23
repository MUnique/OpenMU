// <copyright file="AddDoppelgangerDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the doppelganger event configuration to an existing Season 6 database.
/// </summary>
/// <remarks>
/// It adds the Mirror of Dimensions and the Doppelganger Free Ticket as entrance tickets, the
/// Sign of Dimensions which drops from monsters, the monsters, the reward chests and the <see cref="MiniGameDefinition"/>s,
/// opens the entrance window when talking to Lugard, and lets players who die inside the
/// event maps respawn at Elvenland. Additionally, it reduces the entrance gates of the event maps
/// to their walkable part, because players who got placed on a non-walkable coordinate were
/// warped out of the event.
/// </remarks>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("8E4D2B17-6A3F-4C95-9D02-B7E15A6C3F48")]
public class AddDoppelgangerDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Doppelganger data";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the doppelganger event configuration.";

    private const short LugardNumber = 540;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddDoppelgangerData;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 23, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (new EventTicketItems(context, gameConfiguration).CreateDoppelgangerItems() is { } signOfDimensionsDropGroup)
        {
            foreach (var map in gameConfiguration.Maps)
            {
                map.DropItemGroups.Add(signOfDimensionsDropGroup);
            }
        }

        if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == LugardNumber) is { } lugard)
        {
            lugard.NpcWindow = NpcWindow.LugardDoppelgangerEntry;
        }

        var elvenland = gameConfiguration.Maps.FirstOrDefault(map => map.Number == Elvenland.Number && map.Discriminator == 0);
        foreach (var map in gameConfiguration.Maps.Where(map => map.Number is >= Doppelgaenger1.Number and <= Doppelgaenger4.Number))
        {
            map.SafezoneMap = elvenland;
            if (map.ExitGates.FirstOrDefault(gate => gate.IsSpawnGate) is { } entrance)
            {
                (entrance.X1, entrance.Y1, entrance.X2, entrance.Y2) = GetWalkableEntranceArea(map.Number);
            }
        }

        if (!gameConfiguration.Monsters.Any(monster => monster.Number == DoppelgangerMonsters.FirstMonsterNumber))
        {
            new DoppelgangerMonsters(context, gameConfiguration).Initialize();
        }
        else
        {
            new DoppelgangerMonsters(context, gameConfiguration).ConfigureRewardChests();
        }

        if (!gameConfiguration.MiniGameDefinitions.Any(definition => definition.Type == MiniGameType.Doppelganger))
        {
            new DoppelgangerInitializer(context, gameConfiguration).Initialize();
        }

        // The event accepts two different tickets, which are checked by the EnterDoppelgangerAction.
        foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(definition => definition.Type == MiniGameType.Doppelganger))
        {
            definition.TicketItem = null;
        }

        return ValueTask.CompletedTask;
    }

    private static (byte X1, byte Y1, byte X2, byte Y2) GetWalkableEntranceArea(short mapNumber)
    {
        return mapNumber switch
        {
            Doppelgaenger1.Number => (194, 26, 199, 32),
            Doppelgaenger2.Number => (134, 69, 139, 74),
            Doppelgaenger3.Number => (106, 60, 111, 62),
            Doppelgaenger4.Number => (92, 13, 97, 17),
            _ => throw new ArgumentOutOfRangeException(nameof(mapNumber), mapNumber, null),
        };
    }
}
