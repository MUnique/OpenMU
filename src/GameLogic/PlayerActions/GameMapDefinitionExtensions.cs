// <copyright file="GameMapDefinitionExtensions.cs" company="MUnique">
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
    /// Checks if the map is active and the <see cref="GameMapDefinition.MapRequirements"/> for the specified player.
    /// </summary>
    /// <param name="gameMapDefinition">The game map definition.</param>
    /// <param name="player">The player.</param>
    /// <param name="errorMessage">The error message, which is available when this method returns <c>true</c>.</param>
    /// <returns><c>False</c>, if the requirements are fulfilled; Otherwise, <c>true</c>.</returns>
    public static bool TryGetRequirementError(this GameMapDefinition gameMapDefinition, Player player, [MaybeNullWhen(false)] out string errorMessage)
    {
        errorMessage = null;

        if (!gameMapDefinition.IsActive)
        {
            errorMessage = player.GetLocalizedMessage(nameof(PlayerMessage.MapIsNotActive));
            return true;
        }

        if (gameMapDefinition.MapRequirements is null || !gameMapDefinition.MapRequirements.Any())
        {
            return false;
        }

        foreach (var requirement in gameMapDefinition.MapRequirements)
        {
            if (player.Attributes is null || player.Attributes[requirement.Attribute] < requirement.MinimumValue)
            {
                errorMessage = player.GetLocalizedMessage(PlayerMessage.MissingMapRequirement, requirement.Attribute?.Description);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the active map to which a character can be spawned: the first of the candidates which is
    /// active and has a spawn gate. If none of them is suitable, e.g. because the home map of the
    /// character class is inactive, it's the active map with the lowest number which has a spawn gate,
    /// so that the character doesn't get stuck on content which isn't available in the game.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <param name="candidates">The candidates, in the order of their preference.</param>
    /// <returns>The active map, or <c>null</c> if no map is active and has a spawn gate.</returns>
    public static GameMapDefinition? GetActiveSpawnMap(this GameConfiguration configuration, params GameMapDefinition?[] candidates)
    {
        return candidates.FirstOrDefault(IsActiveSpawnMap)
               ?? configuration.Maps.Where(IsActiveSpawnMap).OrderBy(map => map.Number).ThenBy(map => map.Discriminator).FirstOrDefault();

        static bool IsActiveSpawnMap(GameMapDefinition? map) => map is { IsActive: true } && map.ExitGates.Any(gate => gate.IsSpawnGate);
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