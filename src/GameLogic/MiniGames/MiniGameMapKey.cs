// <copyright file="MiniGameMapKey.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// Defines a key for a <see cref="MiniGameContext"/>, which is unique within a <see cref="IGameContext"/>.
/// </summary>
/// <param name="MapNumber">The number of the map, <see cref="GameMapDefinition.Number"/>.</param>
/// <param name="MiniGameLevel">The level of the mini game, <see cref="MiniGameDefinition.GameLevel"/>.</param>
/// <param name="Owner">
/// The owner of the mini game, <see cref="Character.Name"/>.
/// When the owner is a party, it contains the <see cref="Character.Name"/> of the party leader.
/// Empty, if the instance of the game is not assigned to a player or party.
/// </param>
public record MiniGameMapKey(short MapNumber, byte MiniGameLevel, string Owner)
{
    /// <summary>
    /// Creates a new instance of a <see cref="MiniGameMapKey"/>.
    /// </summary>
    /// <param name="definition">The mini game definition.</param>
    /// <param name="requester">The requester of the map.</param>
    /// <returns>The created key for the mini game.</returns>
    public static MiniGameMapKey Create(MiniGameDefinition definition, Player requester)
    {
        var mapDefinition = definition.Entrance?.Map ?? throw new ArgumentException($"{nameof(definition)} contains no entrance map.");
        MiniGameMapKey miniGameKey;
        switch (definition.MapCreationPolicy)
        {
            case MiniGameMapCreationPolicy.OnePerPlayer:
                miniGameKey = new MiniGameMapKey(mapDefinition.Number, definition.GameLevel, requester.Name);
                break;
            case MiniGameMapCreationPolicy.OnePerParty:
                miniGameKey = new MiniGameMapKey(mapDefinition.Number, definition.GameLevel, requester.Party?.PartyMaster?.Name ?? requester.Name);
                break;
            case MiniGameMapCreationPolicy.Shared:
                miniGameKey = new MiniGameMapKey(mapDefinition.Number, definition.GameLevel, string.Empty);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return miniGameKey;
    }
}