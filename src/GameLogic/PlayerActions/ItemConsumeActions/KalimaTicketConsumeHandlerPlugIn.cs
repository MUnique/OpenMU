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
            || player.CurrentMap is not { } currentMap)
        {
            return false;
        }

        if (KalimaGate.IsKalimaMap(currentMap.MapId))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NoKalimaGateInKalima)).ConfigureAwait(false);
            return false;
        }

        var configuration = this.Configuration ??= new KalimaTicketConsumeHandlerConfiguration();
        var minimumLevels = character.IsSpecialCharacter() ? configuration.SpecialCharacterMinimumLevels : configuration.MinimumLevels;
        var kalimaLevel = GetKalimaLevel(minimumLevels, (int)player.Attributes![Stats.Level]);
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

    /// <summary>
    /// Gets the highest level of Kalima whose minimum character level the character reached.
    /// The minimum levels don't need to be ordered, so a lower level of Kalima with a higher minimum doesn't count.
    /// </summary>
    /// <param name="minimumLevels">The minimum character levels of the levels of Kalima, starting at the first level.</param>
    /// <param name="characterLevel">The character level.</param>
    /// <returns>The level of Kalima, or 0 if the character didn't reach any minimum level.</returns>
    private static int GetKalimaLevel(IList<int> minimumLevels, int characterLevel)
    {
        var kalimaLevel = 0;
        for (var index = 0; index < Math.Min(minimumLevels.Count, KalimaGate.MaximumLevel); index++)
        {
            if (characterLevel >= minimumLevels[index])
            {
                kalimaLevel = index + 1;
            }
        }

        return kalimaLevel;
    }
}
