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
public class SantaVillageInvitationConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn, ISupportCustomConfiguration<SantaVillageInvitationConsumeHandlerConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public SantaVillageInvitationConsumeHandlerConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.InvitationToSantaVillage;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        var mapNumber = (this.Configuration ??= new SantaVillageInvitationConsumeHandlerConfiguration()).MapNumber;
        if (!this.CheckPreconditions(player, item)
            || player.CurrentMap?.MapId == mapNumber
            || await GetSpawnGateAsync(player, mapNumber).ConfigureAwait(false) is not { } spawnGate)
        {
            return false;
        }

        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        await player.WarpToAsync(spawnGate).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new SantaVillageInvitationConsumeHandlerConfiguration();

    /// <summary>
    /// Gets the spawn gate of the safezone of the target map, if the map is active.
    /// The map may be hosted by another game server, so the gate is taken from its definition when
    /// this game server doesn't host it. The warp then moves the player to the hosting game server.
    /// </summary>
    private static async ValueTask<ExitGate?> GetSpawnGateAsync(Player player, short mapNumber)
    {
        if (player.GameContext.Configuration.Maps.FirstOrDefault(map => map.Number == mapNumber) is not { IsActive: true } mapDefinition)
        {
            return null;
        }

        if (await player.GameContext.GetMapAsync((ushort)mapNumber).ConfigureAwait(false) is { } map)
        {
            return map.SafeZoneSpawnGate;
        }

        return mapDefinition.GetSafezoneGate();
    }
}
