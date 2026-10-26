// <copyright file="GameMasterActions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Administration;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

/// <summary>GM actions shared by chat commands and the live administration channel.</summary>
/// <remarks>The administration channel serializes player mutations with the player's persistence lock. Chat commands retain their packet handler context; acquiring a second player's lock there would violate the lock order.</remarks>
public static class GameMasterActions
{
    /// <summary>Sets a level and publishes the same notifications as the GM commands.</summary>
    /// <param name="player">The live player.</param>
    /// <param name="level">The new level.</param>
    /// <param name="master">Whether to set master level.</param>
    /// <returns>The action result.</returns>
    public static async ValueTask<GameServerAdminResult> SetLevelAsync(Player player, int level, bool master)
    {
        var maximum = master ? player.GameContext.Configuration.MaximumMasterLevel : player.GameContext.Configuration.MaximumLevel;
        if (player.SelectedCharacter is null || player.Attributes is not { } attributes)
        {
            return new(false, "The live character is unavailable. Refresh players_list.");
        }

        if (level < 1 || level > maximum)
        {
            return new(false, "The level is outside the configured range.");
        }

        attributes[master ? Stats.MasterLevel : Stats.Level] = level;
        if (master)
        {
            await player.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateMasterLevelAsync()).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateLevelAsync()).ConfigureAwait(false);
        }

        await player.ForEachWorldObserverAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(player, IShowEffectPlugIn.EffectType.LevelUp), true).ConfigureAwait(false);
        return GameServerAdminResult.Success;
    }

    /// <summary>Moves a character to a walkable coordinate, using the normal map transition.</summary>
    /// <param name="player">The live player.</param>
    /// <param name="mapNumber">The target map number.</param>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    /// <returns>The action result.</returns>
    public static async ValueTask<GameServerAdminResult> TeleportAsync(Player player, ushort mapNumber, byte x, byte y)
    {
        if (player.CurrentMap is not { } current || !player.IsAlive || player.IsTeleporting || player.CurrentMiniGame is not null)
        {
            return new(false, "The character is changing maps, dead or in a mini game. Wait for a stable live session.");
        }

        // Looking up an existing map avoids creating an instance or routing to another server.
        var map = current.MapId == mapNumber ? current : await player.GameContext.GetMapAsync(mapNumber, false).ConfigureAwait(false);
        if (map is null || !map.Terrain.WalkMap[x, y])
        {
            return new(false, "The destination is blocked or not hosted by this game server.");
        }

        if (map == current)
        {
            await MoveAsync(player, new Point(x, y)).ConfigureAwait(false);
        }
        else
        {
            await player.WarpToAsync(new ExitGate { Map = map.Definition, X1 = x, X2 = x, Y1 = y, Y2 = y }).ConfigureAwait(false);
        }

        return player.SelectedCharacter?.CurrentMap == map.Definition && player.Position == new Point(x, y)
            ? GameServerAdminResult.Success : new(false, "The map transition was refused. Refresh the live character before retrying.");
    }

    /// <summary>Inserts one validated item into the live inventory and notifies the client.</summary>
    /// <param name="player">The live player.</param>
    /// <param name="specification">The requested item.</param>
    /// <returns>The action result.</returns>
    public static async ValueTask<GameServerAdminResult> GiveItemAsync(Player player, ItemSpecification specification)
    {
        var definition = player.GameContext.Configuration.Items.SingleOrDefault(d => d.Group == specification.Group && d.Number == specification.Number);
        if (definition is null || !GameMasterItemFactory.IsValid(definition, specification))
        {
            return new(false, "The item or one of its options is outside its configured definition.");
        }

        if (player.Inventory is not { } inventory)
        {
            return new(false, "The character has no live inventory.");
        }

        var temporary = GameMasterItemFactory.CreateItem(definition, specification);
        if (inventory.CheckInvSpace(temporary) is not { } slot)
        {
            return new(false, "The live inventory has no free rectangle for this item. Free space first.");
        }

        // Persist in the player's own context, including extension slots. Never save through an offline context.
        var item = temporary.MakePersistent(player.PersistenceContext);
        if (!await inventory.AddItemAsync(slot, item).ConfigureAwait(false))
        {
            player.PersistenceContext.Detach(item);
            return new(false, "The inventory changed before the item could be inserted. Inspect it before retrying.");
        }

        await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
        return GameServerAdminResult.Success;
    }

    /// <summary>Performs the instantaneous same-map movement used by the GM teleport command.</summary>
    /// <param name="player">The player on the current map.</param>
    /// <param name="target">The target coordinate.</param>
    /// <returns>The movement completion.</returns>
    internal static ValueTask MoveAsync(Player player, Point target) => player.MoveAsync(target);
}
