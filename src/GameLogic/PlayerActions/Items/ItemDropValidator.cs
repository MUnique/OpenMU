// <copyright file="ItemDropValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Collections.Concurrent;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Validator for item drop operations to prevent exploits and cheating.
/// </summary>
public class ItemDropValidator
{
    private const int MaxAllowedDropDistance = 5; // Maximum tiles from player position
    private const int MinDropCooldownMs = 100; // Minimum time between drops (anti-spam)
    private const int MaxDropsPerSecond = 10; // Rate limiting
    private const int MaxDropsPerMinute = 100; // Burst protection
    
    private readonly ConcurrentDictionary<string, List<DateTime>> _playerDropHistory = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastDropTime = new();

    /// <summary>
    /// Validates if an item drop operation is allowed.
    /// </summary>
    /// <param name="player">The player performing the drop.</param>
    /// <param name="item">The item being dropped.</param>
    /// <param name="target">The target position for the drop.</param>
    /// <returns>A validation result indicating if the drop is allowed and any error message.</returns>
    public ItemDropValidationResult ValidateItemDrop(Player player, Item item, Point target)
    {
        // Basic null checks
        if (player?.CurrentMap?.Terrain == null)
        {
            return new ItemDropValidationResult(false, "Invalid player or map state");
        }

        if (item?.Definition == null)
        {
            return new ItemDropValidationResult(false, "Invalid item");
        }

        // Check if target position is within map bounds
        var terrain = player.CurrentMap.Terrain;
        if (target.X < 0 || target.Y < 0 ||
            target.X >= terrain.WalkMap.GetLength(0) ||
            target.Y >= terrain.WalkMap.GetLength(1))
        {
            return new ItemDropValidationResult(false, "Target position is outside map bounds");
        }

        // Check if target position is walkable
        if (!terrain.WalkMap[target.X, target.Y])
        {
            return new ItemDropValidationResult(false, "Cannot drop item on unwalkable terrain");
        }

        // Validate drop distance from player
        var playerPosition = player.IsWalking ? player.WalkTarget : player.Position;
        var distance = playerPosition.EuclideanDistanceTo(target);
        if (distance > MaxAllowedDropDistance)
        {
            return new ItemDropValidationResult(false, $"Drop distance ({distance:F1}) exceeds maximum allowed ({MaxAllowedDropDistance})");
        }

        // Check rate limiting
        var rateLimitResult = this.ValidateRateLimit(player);
        if (!rateLimitResult.IsValid)
        {
            return rateLimitResult;
        }

        // Check item-specific validations
        var itemValidationResult = this.ValidateItemSpecific(player, item);
        if (!itemValidationResult.IsValid)
        {
            return itemValidationResult;
        }

        // Update drop history for rate limiting
        this.UpdateDropHistory(player);

        return new ItemDropValidationResult(true, null);
    }

    /// <summary>
    /// Cleans up old drop history entries to prevent memory leaks.
    /// </summary>
    public void CleanupOldEntries()
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-5);
        var playersToRemove = new List<string>();

        foreach (var kvp in this._playerDropHistory)
        {
            lock (kvp.Value)
            {
                kvp.Value.RemoveAll(dt => dt < cutoffTime);
                if (kvp.Value.Count == 0)
                {
                    playersToRemove.Add(kvp.Key);
                }
            }
        }

        foreach (var playerId in playersToRemove)
        {
            this._playerDropHistory.TryRemove(playerId, out _);
            this._lastDropTime.TryRemove(playerId, out _);
        }
    }

    /// <summary>
    /// Validates rate limiting for item drops to prevent spam.
    /// </summary>
    /// <param name="player">The player being validated.</param>
    /// <returns>Validation result for rate limiting.</returns>
    private ItemDropValidationResult ValidateRateLimit(Player player)
    {
        var playerId = player.Id.ToString();
        var now = DateTime.UtcNow;

        // Check minimum cooldown between drops
        if (this._lastDropTime.TryGetValue(playerId, out var lastDrop))
        {
            var timeSinceLastDrop = now - lastDrop;
            if (timeSinceLastDrop.TotalMilliseconds < MinDropCooldownMs)
            {
                return new ItemDropValidationResult(false, "Drop cooldown not met - please wait before dropping another item");
            }
        }

        // Get or create drop history for this player
        var dropHistory = this._playerDropHistory.GetOrAdd(playerId, _ => new List<DateTime>());

        lock (dropHistory)
        {
            // Remove entries older than 1 minute
            dropHistory.RemoveAll(dt => (now - dt).TotalMinutes > 1);

            // Check drops per minute limit
            if (dropHistory.Count >= MaxDropsPerMinute)
            {
                return new ItemDropValidationResult(false, "Too many items dropped in the last minute");
            }

            // Check drops per second limit
            var dropsInLastSecond = dropHistory.Count(dt => (now - dt).TotalSeconds <= 1);
            if (dropsInLastSecond >= MaxDropsPerSecond)
            {
                return new ItemDropValidationResult(false, "Too many items dropped in the last second");
            }
        }

        return new ItemDropValidationResult(true, null);
    }

    /// <summary>
    /// Validates item-specific drop rules.
    /// </summary>
    /// <param name="player">The player dropping the item.</param>
    /// <param name="item">The item being dropped.</param>
    /// <returns>Validation result for item-specific rules.</returns>
    private ItemDropValidationResult ValidateItemSpecific(Player player, Item item)
    {
        // Check if item is bound to character
        if (item.Definition?.IsBoundToCharacter == true)
        {
            return new ItemDropValidationResult(false, "Bound items cannot be dropped voluntarily");
        }

        // Validate item durability - prevent dropping broken items to duplicate
        if (item.Durability <= 0 && item.Definition?.Durability > 0)
        {
            return new ItemDropValidationResult(false, "Broken items cannot be dropped");
        }

        // Check if player has permission to drop this item type
        if (item.Definition?.Group == 15) // Jewels
        {
            // Additional validation for valuable items
            var playerLevel = (int)(player.Attributes?[Stats.Level] ?? 0);
            if (playerLevel < 50 && item.Definition.Number >= 13) // High-tier jewels
            {
                return new ItemDropValidationResult(false, "Low-level players cannot drop high-tier jewels");
            }
        }

        return new ItemDropValidationResult(true, null);
    }

    /// <summary>
    /// Updates the drop history for a player.
    /// </summary>
    /// <param name="player">The player whose history to update.</param>
    private void UpdateDropHistory(Player player)
    {
        var playerId = player.Id.ToString();
        var now = DateTime.UtcNow;

        this._lastDropTime[playerId] = now;

        var dropHistory = this._playerDropHistory.GetOrAdd(playerId, _ => new List<DateTime>());
        lock (dropHistory)
        {
            dropHistory.Add(now);
        }
    }
}