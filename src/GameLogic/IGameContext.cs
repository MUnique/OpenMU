﻿// <copyright file="IGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The context of the game.
/// </summary>
public interface IGameContext
{
    /// <summary>
    /// Occurs when a game map got created.
    /// </summary>
    event EventHandler<GameMap>? GameMapCreated;

    /// <summary>
    /// Occurs when a game map got removed.
    /// </summary>
    event EventHandler<GameMap>? GameMapRemoved;

    /// <summary>
    /// Gets the global experience rate.
    /// </summary>
    float ExperienceRate { get; }

    /// <summary>
    /// Gets the repository manager. Used to retrieve data, e.g. from a database.
    /// </summary>
    IPersistenceContextProvider PersistenceContextProvider { get; }

    /// <summary>
    /// Gets the item power up factory.
    /// </summary>
    IItemPowerUpFactory ItemPowerUpFactory { get; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    GameConfiguration Configuration { get; }

    /// <summary>
    /// Gets the plug in manager.
    /// </summary>
    PlugInManager PlugInManager { get; }

    /// <summary>
    /// Gets the feature plug ins.
    /// </summary>
    FeaturePlugInContainer FeaturePlugIns { get; }

    /// <summary>
    /// Gets the players count of the game.
    /// </summary>
    int PlayerCount { get; }

    /// <summary>
    /// Gets the logger factory.
    /// </summary>
    ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Gets the drop generator.
    /// </summary>
    IDropGenerator DropGenerator { get; }

    /// <summary>
    /// Gets the initialized maps which are hosted on this context.
    /// </summary>
    IEnumerable<GameMap> Maps { get; }

    /// <summary>
    /// Adds the player to the game.
    /// </summary>
    /// <param name="player">The player.</param>
    void AddPlayer(Player player);

    /// <summary>
    /// Gets the maps which is meant to be hosted by the game.
    /// </summary>
    /// <param name="mapId">The map identifier.</param>
    /// <param name="createIfNotExists">If set to <c>true</c>, the map is created if it doesn't exist yet.</param>
    /// <returns>
    /// The hosted GameMap instance.
    /// </returns>
    GameMap? GetMap(ushort mapId, bool createIfNotExists = true);

    /// <summary>
    /// Gets the mini game map which is meant to be hosted by the game.
    /// </summary>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <param name="requester">The requesting player.</param>
    /// <returns>
    /// The state of the mini game which contains the hosted GameMap instance.
    /// </returns>
    MiniGameContext GetMiniGame(MiniGameDefinition miniGameDefinition, Player requester);

    /// <summary>
    /// Removes the mini game instance from the context.
    /// </summary>
    /// <param name="miniGameContext">The context of the mini game.</param>
    void RemoveMiniGame(MiniGameContext miniGameContext);

    /// <summary>
    /// Gets the player object by character name.
    /// </summary>
    /// <param name="name">The character name.</param>
    /// <returns>The player object.</returns>
    Player? GetPlayerByCharacterName(string name);

    /// <summary>
    /// Sends a global message to all players of the game with the specified message type.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    void SendGlobalMessage(string message, MessageType messageType);

    /// <summary>
    /// Sends a golden global notification to all players of the game.
    /// </summary>
    /// <param name="message">The message.</param>
    void SendGlobalNotification(string message);

    /// <summary>
    /// Executes an action for each player.
    /// </summary>
    /// <param name="action">The action which is executed.</param>
    /// <remarks>
    /// Please avoid doing actions which may lead to the connected-state of the players.
    /// </remarks>
    void ForEachPlayer(Action<Player> action);
}