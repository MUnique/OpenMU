// <copyright file="KalimaTicketConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the kalima ticket, which opens a gate to the level of Kalima which fits the level of the character.
/// Its durability is the number of usages.
/// </summary>
[Guid("F08D7B62-9E78-4004-A2A9-7EA2D0F44B9A")]
[PlugIn]
[Display(Name = nameof(PlugInResources.KalimaTicketConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.KalimaTicketConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class KalimaTicketConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn, ISupportCustomConfiguration<KalimaTicketConsumeHandlerConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public KalimaTicketConsumeHandlerConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.KalimaTicket;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item)
            || player.SelectedCharacter is not { } character
            || player.CurrentMap is not { } currentMap
            || KalimaGate.IsKalimaMap(currentMap.MapId))
        {
            return false;
        }

        var configuration = this.Configuration ??= new KalimaTicketConsumeHandlerConfiguration();
        var minimumLevels = character.IsSpecialCharacter() ? configuration.SpecialCharacterMinimumLevels : configuration.MinimumLevels;
        var characterLevel = (int)player.Attributes![Stats.Level];
        var kalimaLevel = Math.Min(minimumLevels.Count(minimumLevel => characterLevel >= minimumLevel), KalimaGate.MaximumLevel);
        if (kalimaLevel == 0)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LevelTooLowToEnterMap)).ConfigureAwait(false);
            return false;
        }

        if (!await KalimaGate.TryOpenAsync(player, kalimaLevel, player.Position).ConfigureAwait(false))
        {
            return false;
        }

        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new KalimaTicketConsumeHandlerConfiguration();
}
