// <copyright file="TalismanOfMobilityConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Talisman of Mobility. It has two usages: the first one saves the place of the character,
/// the second one warps the character back to the saved place.
/// </summary>
/// <remarks>
/// The game client describes it as "Saves the location with the right-mouse-click. Returns to the saved location by a click.".
/// The place is saved at the character, so it's kept when the character leaves the game.
/// It can't be used in mini games and duels, because these have their own maps and spawn places.
/// </remarks>
[Guid("23071045-4323-4B5F-B26B-5283EEB130C1")]
[PlugIn]
[Display(Name = nameof(PlugInResources.TalismanOfMobilityConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.TalismanOfMobilityConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class TalismanOfMobilityConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <summary>
    /// The durability of an unused talisman. With it, the place is saved; with less, the character returns to it.
    /// </summary>
    private const double UnusedDurability = 2;

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.TalismanOfMobility;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item)
            || player.SelectedCharacter is not { } character
            || player.CurrentMap is not { } currentMap
            || player.CurrentMiniGame is not null
            || player.DuelRoom is not null)
        {
            return false;
        }

        if (item.Durability >= UnusedDurability)
        {
            var position = player.Position;
            SetValue(player, character, Stats.MobilityTalismanMap, currentMap.MapId + 1);
            SetValue(player, character, Stats.MobilityTalismanX, position.X);
            SetValue(player, character, Stats.MobilityTalismanY, position.Y);
            await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MobilityTalismanPlaceSaved)).ConfigureAwait(false);
            return true;
        }

        if (await GetSavedGateAsync(player, character).ConfigureAwait(false) is not { } gate)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MobilityTalismanNoPlaceSaved)).ConfigureAwait(false);
            return false;
        }

        SetValue(player, character, Stats.MobilityTalismanMap, 0);
        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        await player.WarpToAsync(gate).ConfigureAwait(false);
        return true;
    }

    private static async ValueTask<ExitGate?> GetSavedGateAsync(Player player, Character character)
    {
        var mapNumber = (int)GetValue(character, Stats.MobilityTalismanMap) - 1;
        if (mapNumber < 0
            || player.GameContext.Configuration.Maps.FirstOrDefault(map => map.Number == mapNumber) is not { IsActive: true } mapDefinition)
        {
            return null;
        }

        var x = (byte)GetValue(character, Stats.MobilityTalismanX);
        var y = (byte)GetValue(character, Stats.MobilityTalismanY);

        // The map may be hosted by another game server; the warp then moves the player to it.
        var map = await player.GameContext.GetMapAsync((ushort)mapNumber).ConfigureAwait(false);
        return new ExitGate
        {
            Map = map?.Definition ?? mapDefinition,
            X1 = x,
            X2 = x,
            Y1 = y,
            Y2 = y,
        };
    }

    private static float GetValue(Character character, AttributeDefinition attribute)
    {
        return character.Attributes.FirstOrDefault(a => a.Definition?.Id == attribute.Id)?.Value ?? 0;
    }

    private static void SetValue(Player player, Character character, AttributeDefinition attribute, float value)
    {
        if (character.Attributes.FirstOrDefault(a => a.Definition?.Id == attribute.Id) is { } existing)
        {
            existing.Value = value;
            return;
        }

        var definition = player.GameContext.Configuration.Attributes?.FirstOrDefault(a => a.Id == attribute.Id) ?? attribute;
        character.Attributes.Add(player.PersistenceContext.CreateNew<StatAttribute>(definition, value));
    }
}
