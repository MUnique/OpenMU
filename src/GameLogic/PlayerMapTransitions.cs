// <copyright file="PlayerMapTransitions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The map transitions of a <see cref="Player"/>: teleporting, warping, respawning, and the
/// handshake with the game client which loads the new map.
/// </summary>
internal sealed class PlayerMapTransitions
{
    private readonly Player _player;

    private readonly PlayerMovement _movement;

    private readonly PlayerSummon _summon;

    /// <summary>
    /// Whether a <see cref="RecoverFromBlockedSpawnAsync"/> is currently in progress, so that a
    /// nested one does not warp again. See there for why that recursion is fatal.
    /// </summary>
    private bool _isRecoveringFromBlockedSpawn;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerMapTransitions"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="movement">The movement of the player.</param>
    /// <param name="summon">The summon of the player.</param>
    public PlayerMapTransitions(Player player, PlayerMovement movement, PlayerSummon summon)
    {
        this._player = player;
        this._movement = movement;
        this._summon = summon;
    }

    /// <summary>
    /// Teleports the player to the specified target with the specified skill animation.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="teleportSkill">The teleport skill.</param>
    public async Task TeleportAsync(Point target, Skill teleportSkill)
    {
        var player = this._player;
        if (!player.IsAlive)
        {
            return;
        }

        player.IsTeleporting = true;
        try
        {
            await (player.SkillCancelTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);

            await this._movement.StopWalkingAsync().ConfigureAwait(false);
            await this._movement.ResetMovementStateAsync().ConfigureAwait(false);

            var previous = player.Position;
            player.Position = target;

            await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, player, teleportSkill, true), true).ConfigureAwait(false);

            await Task.Delay(300).ConfigureAwait(false);

            await player.ForEachWorldObserverAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(player.GetAsEnumerable()), false).ConfigureAwait(false);

            await Task.Delay(1500).ConfigureAwait(false);

            if (player.IsAlive)
            {
                await player.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);

                // We need to restore the previous position to make the Moving on the map data structure work correctly.
                player.Position = previous;
                if (player.CurrentMap is { } map)
                {
                    await this._movement.MoveOnMapAsync(map, target, MoveType.Teleport).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e)
        {
            player.Logger.LogWarning(e, "Error during teleport");
        }

        player.IsTeleporting = false;
    }

    /// <summary>
    /// Teleports the player to the specified target map and point.
    /// </summary>
    /// <param name="targetMap">The target map for teleportation.</param>
    /// <param name="targetPoint">The target coordinate in the target map.</param>
    public async Task TeleportToMapAsync(GameMap targetMap, Point targetPoint)
    {
        var player = this._player;
        if (!player.IsAlive)
        {
            return;
        }

        player.IsTeleporting = true;
        try
        {
            await (player.SkillCancelTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);

            await this._movement.StopWalkingAsync().ConfigureAwait(false);

            await player.ForEachWorldObserverAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(player.GetAsEnumerable()), false).ConfigureAwait(false);

            if (player.IsAlive)
            {
                ExitGate tempGate = new()
                {
                    Map = targetMap.Definition,
                    X1 = targetPoint.X,
                    X2 = targetPoint.X,
                    Y1 = targetPoint.Y,
                    Y2 = targetPoint.Y,
                };

                await this.WarpToAsync(tempGate).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            player.Logger.LogWarning(e, "Error during teleport");
        }

        player.IsTeleporting = false;
    }

    /// <summary>
    /// Moves the player to the specified gate.
    /// </summary>
    /// <param name="gate">The gate to which the player should be moved.</param>
    public async ValueTask WarpToAsync(ExitGate gate)
    {
        var player = this._player;
        var isRespawnOnSameMap = object.Equals(player.CurrentMap?.Definition, gate.Map);
        if (!await this.TryRemoveFromCurrentMapAsync(isRespawnOnSameMap).ConfigureAwait(false))
        {
            return;
        }

        await this.PlaceAtGateAsync(gate).ConfigureAwait(false);
        player.CurrentMap = null; // Will be set again, when the client acknowledged the map change by F3 12 packet.

        if (!player.PlayerState.CurrentState.IsDisconnectedOrFinished())
        {
            await player.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.ChangingMap).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeAsync()).ConfigureAwait(false);
        }

        // after this, the Client will send us a F3 12 packet, to tell us it loaded
        // the map and is ready to receive the new meet player/monster etc.
        // Then ClientReadyAfterMapChange is called.
    }

    /// <summary>
    /// Moves the player to its spawn gate, usually the safe zone of the current map.
    /// </summary>
    public async ValueTask WarpToSafezoneAsync()
    {
        await this.WarpToAsync(await this.GetSpawnGateOfCurrentMapAsync().ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <summary>
    /// Respawns the player at the specified gate.
    /// </summary>
    /// <param name="gate">The gate at which the player should be respawned.</param>
    public async ValueTask RespawnAtAsync(ExitGate gate)
    {
        var player = this._player;
        var isRespawnOnSameMap = object.Equals(player.CurrentMap?.Definition, gate.Map);

        if (!await this.TryRemoveFromCurrentMapAsync(isRespawnOnSameMap).ConfigureAwait(false))
        {
            return;
        }

        player.ThrowNotInitializedProperty(player.SelectedCharacter is null, nameof(player.SelectedCharacter));
        player.SelectedCharacter.ThrowNotInitializedProperty(player.SelectedCharacter.CurrentMap is null, nameof(player.SelectedCharacter.CurrentMap));
        await this.PlaceAtGateAsync(gate).ConfigureAwait(false);
        player.ClearRespawnAfterDeathToken();

        if (player.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>() is { } respawnPlugIn)
        {
            // Older clients use a separate packet for the respawn, while newer don't.
            // It requires a slightly different logic.
            player.CurrentMap = await player.GameContext.GetMapAsync(player.SelectedCharacter!.CurrentMap!.Number.ToUnsigned()).ConfigureAwait(false) ?? throw new InvalidOperationException("Current map not found.");
            await respawnPlugIn.RespawnAsync().ConfigureAwait(false);
            await player.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
            player.IsAlive = true;
            await player.CurrentMap!.AddAsync(player).ConfigureAwait(false);
        }
        else
        {
            player.CurrentMap = null; // Will be set again, when the client acknowledged the map change by F3 12 packet.
            await player.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.ChangingMap).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeAsync()).ConfigureAwait(false);

            // after this, the Client will send us a F3 12 packet, to tell us it loaded
            // the map and is ready to receive the new meet player/monster etc.
            // Then ClientReadyAfterMapChange is called.
        }
    }

    /// <summary>
    /// Signals that the game client of the player is ready after a map change (data has been loaded etc.).
    /// In this event, the player enters the game map on the server side and interacts with the other objects.
    /// </summary>
    public async ValueTask ClientReadyAfterMapChangeAsync()
    {
        var player = this._player;
        player.ThrowNotInitializedProperty(player.SelectedCharacter is null, nameof(player.SelectedCharacter));
        player.SelectedCharacter.ThrowNotInitializedProperty(player.SelectedCharacter.CurrentMap is null, nameof(player.SelectedCharacter.CurrentMap));

        if (player.CurrentMap is not null)
        {
            // Guard against a repeated F3 12 (client ready after map change) packet.
            // A map change usually leaves CurrentMap null until this handler assigns it,
            // so a non-null value means the handler already ran. The exception is the
            // IRespawnAfterDeathPlugIn branch of RespawnAtAsync, which assigns CurrentMap
            // and adds the player itself; a trailing packet is redundant there as well.
            // Without this guard, a duplicate packet adds the player (and its summon) to
            // the area of interest a second time, which the bucket does not deduplicate.
            player.Logger.LogWarning("Ignoring client-ready packet: player {0} is already on map {1}.", player, player.CurrentMap);
            return;
        }

        if (player.CurrentMiniGame is { } currentMiniGame)
        {
            player.CurrentMap = currentMiniGame.Map;
        }
        else
        {
            player.CurrentMap = await player.GameContext.GetMapAsync(player.SelectedCharacter!.CurrentMap.Number.ToUnsigned()).ConfigureAwait(false);
        }

        await player.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
        player.IsAlive = true;

        await player.CurrentMap!.AddAsync(player).ConfigureAwait(false);
        if (!player.CurrentMap.Terrain.WalkMap[player.SelectedCharacter.PositionX, player.SelectedCharacter.PositionY]
            && await this.RecoverFromBlockedSpawnAsync().ConfigureAwait(false))
        {
            // The warp starts another map change, which adds the summon again when it completed.
            return;
        }

        await this._summon.AddToMapAsync(player.CurrentMap).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the player from its current map, e.g. when it leaves the game.
    /// </summary>
    public async ValueTask RemoveFromCurrentMapAsync()
    {
        var player = this._player;
        if (player.CurrentMap is { } map)
        {
            await map.RemoveAsync(player).ConfigureAwait(false);
            player.SetCurrentMapSilently(null);
        }
    }

    /// <summary>
    /// Gets the gate at which the player is spawned. Plugins of the
    /// <see cref="IPlayerSpawnGateSelectionPlugIn"/> can provide a gate of their game feature;
    /// otherwise, it's the safezone gate of the current map.
    /// </summary>
    /// <returns>The spawn gate.</returns>
    /// <exception cref="InvalidOperationException">The current map is not set, or has no spawn gate.</exception>
    public async ValueTask<ExitGate> GetSpawnGateOfCurrentMapAsync()
    {
        var player = this._player;
        if (player.CurrentMap is null)
        {
            throw new InvalidOperationException("CurrentMap is not set. Can't determine spawn gate.");
        }

        if (player.GameContext.PlugInManager.GetPlugInPoint<IPlayerSpawnGateSelectionPlugIn>() is { } plugInPoint)
        {
            var args = new SpawnGateSelectionArgs();
            await plugInPoint.SelectSpawnGateAsync(player, args).ConfigureAwait(false);
            if (args.Gate is { } selectedGate)
            {
                return selectedGate;
            }
        }

        var spawnTargetMapDefinition = player.CurrentMap.Definition.SafezoneMap ?? player.CurrentMap.Definition;
        var targetMap = await player.GameContext.GetMapAsync((ushort)spawnTargetMapDefinition.Number, false).ConfigureAwait(false);
        return targetMap?.SafeZoneSpawnGate
               ?? spawnTargetMapDefinition.GetSafezoneGate()
               ?? throw new InvalidOperationException($"Game map {spawnTargetMapDefinition} has no spawn gate.");
    }

    private async ValueTask<bool> TryRemoveFromCurrentMapAsync(bool willRespawnOnSameMap)
    {
        var player = this._player;
        var currentMap = player.CurrentMap;
        if (currentMap is null)
        {
            return true;
        }

        if (willRespawnOnSameMap)
        {
            await currentMap.InitRespawnAsync(player).ConfigureAwait(false);
        }
        else
        {
            await currentMap.RemoveAsync(player).ConfigureAwait(false);
        }

        player.IsAlive = false;
        player.IsTeleporting = false;
        await this._movement.StopWalkingAsync().ConfigureAwait(false);
        await player.ClearObservingObjectsListAsync().ConfigureAwait(false);
        await this._summon.RemoveFromMapAsync(currentMap).ConfigureAwait(false);

        return true;
    }

    /// <summary>
    /// Places the player (and its summon) at the specified gate.
    /// </summary>
    /// <param name="gate">The gate.</param>
    internal async ValueTask PlaceAtGateAsync(ExitGate gate)
    {
        var player = this._player;
        player.SelectedCharacter!.PositionX = (byte)Rand.NextInt(gate.X1, gate.X2);
        player.SelectedCharacter.PositionY = (byte)Rand.NextInt(gate.Y1, gate.Y2);
        player.SelectedCharacter.CurrentMap = gate.Map;
        player.Rotation = gate.Direction;

        await this._movement.ResetMovementStateAsync().ConfigureAwait(false);

        this._summon.PlaceAtGate(gate);
    }

    /// <summary>
    /// Recovers a player which was placed on a non-walkable tile, by warping it to its safezone.
    /// That warp re-enters <see cref="ClientReadyAfterMapChangeAsync"/>: for a player with a game
    /// client the re-entry arrives later and on a fresh stack, with its F3 12 packet - but a
    /// connection-less player (an <see cref="Offline.OfflinePlayer"/>, i.e. a bot) gets it inline
    /// from <see cref="Offline.OfflineMapChangePlugIn"/>. Since <see cref="PlaceAtGateAsync"/>
    /// rolls the position within the gate only once and never retries, a safezone spawn gate
    /// without a single walkable tile recursed until the stack overflowed - taking the whole game
    /// server process down with it. The recovery therefore warps at most once per such re-entry: a
    /// nested attempt places the player on a walkable tile of the map it already stands on instead.
    /// This bounds the recursion, which is what took the process down. It does not bound the case of
    /// a player with a game client, whose re-entry arrives on a later, fresh stack with this flag
    /// already reset - such a player can still be warped back and forth between two blocked gates,
    /// exactly as before, which is a stuck client rather than a dead server.
    /// </summary>
    /// <returns>True, if the player was warped; false, if it was placed on this map instead.</returns>
    private async ValueTask<bool> RecoverFromBlockedSpawnAsync()
    {
        var player = this._player;
        if (!this._isRecoveringFromBlockedSpawn)
        {
            this._isRecoveringFromBlockedSpawn = true;
            try
            {
                await this.WarpToSafezoneAsync().ConfigureAwait(false);
                return true;
            }
            finally
            {
                this._isRecoveringFromBlockedSpawn = false;
            }
        }

        if (player.CurrentMap is not { } map)
        {
            return false;
        }

        // No warp here - warping is exactly what would recurse. The terrain of the map we stand on
        // is already parsed, so we just step to a tile we can actually stand on.
        // Not RandomWalkableCoordinate: that samples the monster spawn points, which exclude every
        // safezone tile by construction - it would drop a player who is being recovered into a
        // hunting ground, and report failure on a map which is nothing but safezone.
        var target = map.Terrain.GetWalkableCoordinate(map.SafeZoneSpawnGate) ?? map.Terrain.AnyWalkableCoordinate;
        if (target is not { } point)
        {
            player.Logger.LogError("Map {0} has no walkable tile at all - player {1} stays on a blocked one.", map, player);
            return false;
        }

        player.Logger.LogWarning("Spawn gate of map {0} is blocked - placing player {1} at {2} instead of warping again.", map, player, point);
        await player.MoveAsync(point).ConfigureAwait(false);
        return false;
    }
}
