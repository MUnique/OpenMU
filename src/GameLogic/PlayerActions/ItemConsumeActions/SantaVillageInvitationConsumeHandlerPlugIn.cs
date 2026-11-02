// <copyright file="SantaVillageInvitationConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the invitation to the santa village, which warps the player to the santa village.
/// Its durability is the number of usages.
/// </summary>
[Guid("92A01DF4-6B50-43A5-9137-6FE0A1F92EEB")]
[PlugIn]
[Display(Name = nameof(PlugInResources.SantaVillageInvitationConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.SantaVillageInvitationConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class SantaVillageInvitationConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <summary>
    /// The number of the santa village map. The game client asks the player whether to move to the santa village,
    /// before it uses the invitation.
    /// </summary>
    internal const short SantaVillageMapNumber = 62;

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.InvitationToSantaVillage;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item)
            || player.CurrentMap?.MapId == SantaVillageMapNumber
            || await player.GameContext.GetMapAsync((ushort)SantaVillageMapNumber).ConfigureAwait(false) is not { SafeZoneSpawnGate: { } spawnGate })
        {
            return false;
        }

        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        await player.WarpToAsync(spawnGate).ConfigureAwait(false);
        return true;
    }
}
