// -----------------------------------------------------------------------
// <copyright file="TownPortalScrollConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the town portal scroll.
/// It warps the player to the nearest town.
/// </summary>
/// <remarks>
/// We might need a field for the "nearest" town in the <see cref="GameMapDefinition"/>.
/// <see cref="GameMapDefinition.SafezoneMap"/> might not be suitable.
/// </remarks>
[Guid("825C3110-75F1-4157-A189-15B365B4791E")]
[PlugIn(nameof(TownPortalScrollConsumeHandlerPlugIn), "Plugin which handles the town portal scroll consumption.")]
public class TownPortalScrollConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.TownPortalScroll;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            var targetMapDef = player.CurrentMap!.Definition.SafezoneMap ?? player.SelectedCharacter!.CharacterClass!.HomeMap;
            if (targetMapDef is { }
                && await player.GameContext.GetMapAsync((ushort)targetMapDef.Number).ConfigureAwait(false) is { SafeZoneSpawnGate: { } spawnGate })
            {
                await player.WarpToAsync(spawnGate).ConfigureAwait(false);
                return true;
            }
        }

        return false;
    }
}