// <copyright file="ActorTestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.TestActors;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Builds scripted actors on top of the in-memory game context of the upstream test helpers.
/// </summary>
public static class ActorTestHelper
{
    /// <summary>
    /// The terrain byte of a walkable tile inside a safe zone.
    /// </summary>
    private const byte SafezoneTerrain = 1;

    /// <summary>
    /// The terrain byte of a tile which cannot be walked on.
    /// </summary>
    private const byte BlockedTerrain = 2;

    /// <summary>
    /// The view range of the test context, as in the game's default configuration.
    /// </summary>
    private const byte DefaultInfoRange = 12;

    /// <summary>
    /// Creates an in-memory game context whose (only) map is walkable everywhere, except for the
    /// given special tiles. Must be called before the map is used, because the terrain is read once.
    /// </summary>
    /// <param name="safezoneTiles">The tiles which belong to a safe zone.</param>
    /// <param name="blockedTiles">The tiles which cannot be walked on.</param>
    /// <returns>The game context.</returns>
    public static IGameContext CreateGameContext(IEnumerable<Point>? safezoneTiles = null, IEnumerable<Point>? blockedTiles = null)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();

        // The upstream helper leaves the view range at 0, which would put nothing at all into an
        // actor's view; the default configuration of the game uses 12.
        gameContext.Configuration.InfoRange = DefaultInfoRange;
        var terrain = gameContext.Configuration.Maps.First().TerrainData!;
        foreach (var tile in safezoneTiles ?? [])
        {
            terrain[3 + tile.X + (tile.Y << 8)] = SafezoneTerrain;
        }

        foreach (var tile in blockedTiles ?? [])
        {
            terrain[3 + tile.X + (tile.Y << 8)] = BlockedTerrain;
        }

        return gameContext;
    }

    /// <summary>
    /// Creates an actor and lets it enter the world of the given context, the way
    /// <see cref="ScriptedPlayer.InitializeAsync"/> does it once the account is loaded.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="loginName">The login name the actor is addressed by.</param>
    /// <param name="characterName">The name of its character.</param>
    /// <returns>The actor, in the world.</returns>
    public static async ValueTask<ScriptedPlayer> CreateActorAsync(IGameContext gameContext, string loginName, string characterName)
    {
        var template = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var account = template.Account!;
        var character = template.SelectedCharacter!;
        account.LoginName = loginName;
        character.Name = characterName;

        var actor = new ScriptedPlayer(gameContext);
        var entered = await actor.EnterWorldAsync(account, character).ConfigureAwait(false);
        Assert.That(entered, Is.True, $"the actor '{loginName}' should have entered the world");
        return actor;
    }
}
