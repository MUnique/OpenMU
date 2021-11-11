﻿// <copyright file="GameMapDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Extension methods for <see cref="GameMapDefinition"/>.
/// </summary>
public static class GameMapDefinitionExtensions
{
    /// <summary>
    /// Checks the <see cref="GameMapDefinition.MapRequirements"/> for the specified player.
    /// </summary>
    /// <param name="gameMapDefinition">The game map definition.</param>
    /// <param name="player">The player.</param>
    /// <param name="errorMessage">The error message, which is available when this method returns <c>true</c>.</param>
    /// <returns><c>False</c>, if the requirements are fulfilled; Otherwise, <c>true</c>.</returns>
    public static bool TryGetRequirementError(this GameMapDefinition gameMapDefinition, Player player, [MaybeNullWhen(false)] out string errorMessage)
    {
        errorMessage = null;

        if (gameMapDefinition.MapRequirements != null && gameMapDefinition.MapRequirements.Any())
        {
            foreach (var requirement in gameMapDefinition.MapRequirements)
            {
                if (player.Attributes is null || player.Attributes[requirement.Attribute] < requirement.MinimumValue)
                {
                    errorMessage = $"Missing requirement to enter the map: {requirement.Attribute?.Description}";
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the safezone gate of a map.
    /// </summary>
    /// <param name="gameMapDefinition">The game map definition.</param>
    /// <param name="terrain">The terrain, if available.</param>
    /// <returns>The safezone gate of a map.</returns>
    public static ExitGate? GetSafezoneGate(this GameMapDefinition gameMapDefinition, GameMapTerrain? terrain = null)
    {
        terrain ??= new GameMapTerrain(gameMapDefinition);
        return gameMapDefinition.ExitGates?.FirstOrDefault(g => g.IsSpawnGate && terrain.SafezoneMap[g.X1, g.Y1])
               ?? gameMapDefinition.ExitGates?.FirstOrDefault(g => g.IsSpawnGate);
    }
}