// <copyright file="TalismanOfMobilityConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Talisman of Mobility. It has two usages: the first one saves the place of the character,
/// the second one warps the character back to the saved place.
/// </summary>
/// <remarks>
/// The game client describes it as "Saves the location with the right-mouse-click. Returns to the saved location by a click.".
/// The place is saved at the character, so it's kept when the character leaves the game.
/// It can't be used in mini games and duels, because these have their own maps and spawn places.
/// The character returns only if it still fulfills the requirements of the saved map, e.g. wings for Icarus;
/// otherwise the talisman isn't used up and the place stays saved.
/// While another talisman in the inventory still has its return usage left, no new place can be saved,
/// because the saved place is shared and the other talisman couldn't return anymore.
/// The same applies when an <see cref="IWarpGateEnteringPlugIn"/> denies the return, e.g. while an event map is closed,
/// or when an <see cref="IPlayerTeleportRestrictionPlugIn"/> blocks the way to a place on the same map, e.g. a closed castle gate.
/// </remarks>
[Guid("23071045-4323-4B5F-B26B-5283EEB130C1")]
[PlugIn]
[Display(Name = nameof(PlugInResources.TalismanOfMobilityConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.TalismanOfMobilityConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class TalismanOfMobilityConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn, IItemMovingPlugIn
{
    /// <summary>
    /// The durability of an unused talisman. With it, the place is saved; with less, the character returns to it.
    /// </summary>
    private const double UnusedDurability = 2;

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.TalismanOfMobility;

    /// <inheritdoc />
    public void ItemMoving(Player player, Item item, Storages targetStorage, int slot, CancelEventArgs eventArgs)
    {
        if (targetStorage == Storages.Vault
            && item.Durability is > 0 and < UnusedDurability
            && item.Definition is { } definition
            && new ItemIdentifier(definition.Number, definition.Group) == this.Key)
        {
            // The return location is stored on this character, while the vault is shared.
            eventArgs.Cancel = true;
        }
    }

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
            if (HasOtherHalfUsedTalisman(player, item))
            {
                // The place is saved at the character, so a new place would strand the return of the other talisman.
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MobilityTalismanReturnPending)).ConfigureAwait(false);
                return false;
            }

            var position = player.Position;
            SetValue(player, character, Stats.MobilityTalismanMap, currentMap.MapId + 1);
            SetValue(player, character, Stats.MobilityTalismanX, position.X);
            SetValue(player, character, Stats.MobilityTalismanY, position.Y);
            await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MobilityTalismanPlaceSaved)).ConfigureAwait(false);
            return true;
        }

        if (GetSavedMap(player, character) is not { } savedMap)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MobilityTalismanNoPlaceSaved)).ConfigureAwait(false);
            return false;
        }

        if (savedMap.TryGetRequirementError(player, out var errorMessage))
        {
            // E.g. the character saved a place in Icarus and lost its wings; it keeps the talisman and the saved place.
            await player.ShowBlueMessageAsync(errorMessage).ConfigureAwait(false);
            return false;
        }

        if (await GetSavedGateAsync(player, character, savedMap).ConfigureAwait(false) is not { } gate)
        {
            // E.g. the game server which hosts the map is stopped; without this, the warp would end at the home map.
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MapIsNotActive)).ConfigureAwait(false);
            return false;
        }

        if (player.GameContext.PlugInManager.GetPlugInPoint<IWarpGateEnteringPlugIn>() is { } plugInPoint)
        {
            // The plugin shows the reason to the player; the talisman and the saved place are kept.
            var eventArgs = new CancelEventArgs();
            await plugInPoint.WarpGateEnteringAsync(player, gate, eventArgs).ConfigureAwait(false);
            if (eventArgs.Cancel)
            {
                return false;
            }
        }

        if (savedMap.Number == currentMap.Definition.Number
            && player.GameContext.PlugInManager.GetPlugInPoint<IPlayerTeleportRestrictionPlugIn>() is { } restrictionPoint)
        {
            // On the same map, the return must not bypass what blocks a teleport, e.g. closed castle gates.
            var eventArgs = new CancelEventArgs();
            restrictionPoint.CheckTeleport(player, new Point(gate.X1, gate.Y1), eventArgs);
            if (eventArgs.Cancel)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MobilityTalismanReturnBlocked)).ConfigureAwait(false);
                return false;
            }
        }

        SetValue(player, character, Stats.MobilityTalismanMap, 0);
        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        await player.WarpToAsync(gate).ConfigureAwait(false);
        return true;
    }

    private static bool HasOtherHalfUsedTalisman(Player player, Item item)
    {
        return player.Inventory?.Items.Any(other => other != item
                                                    && other.Definition == item.Definition
                                                    && other.Durability is > 0 and < UnusedDurability) ?? false;
    }

    private static GameMapDefinition? GetSavedMap(Player player, Character character)
    {
        var mapNumber = (int)GetValue(character, Stats.MobilityTalismanMap) - 1;
        return mapNumber < 0
            ? null
            : player.GameContext.Configuration.Maps.FirstOrDefault(map => map.Number == mapNumber);
    }

    private static async ValueTask<ExitGate?> GetSavedGateAsync(Player player, Character character, GameMapDefinition mapDefinition)
    {
        var x = (byte)GetValue(character, Stats.MobilityTalismanX);
        var y = (byte)GetValue(character, Stats.MobilityTalismanY);

        // The map may be hosted by another game server; the warp then moves the player to it.
        var mapNumber = (ushort)mapDefinition.Number;
        var map = await player.GameContext.GetMapAsync(mapNumber).ConfigureAwait(false);
        if (map is null
            && player.GameContext is IGameServerContext { MapHostLocator: { } locator } serverContext)
        {
            map = (await locator.LocateAsync(serverContext, mapNumber).ConfigureAwait(false))?.Map;
        }

        if (map is null)
        {
            return null;
        }

        return new ExitGate
        {
            Map = map.Definition,
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
