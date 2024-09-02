// <copyright file="IGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
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
    /// Gets a value indicating whether PVP is enabled.
    /// </summary>
    bool PvpEnabled { get; }

    /// <summary>
    /// Gets the repository provider. Used to retrieve data, e.g. from a database.
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
    /// Gets the experience table. Index is the player level, value the needed experience to reach that level.
    /// </summary>
    long[] ExperienceTable { get; }

    /// <summary>
    /// Gets the master experience table. Index is the player level, value the needed experience to reach that level.
    /// </summary>
    long[] MasterExperienceTable { get; }

    /// <summary>
    /// Gets the configuration change mediator.
    /// </summary>
    IConfigurationChangeMediator ConfigurationChangeMediator { get; }

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
    /// Gets the object pool for path finders.
    /// </summary>
    IObjectPool<PathFinder> PathFinderPool { get; }

    /// <summary>
    /// Gets the duel room manager.
    /// </summary>
    DuelRoomManager DuelRoomManager { get; }

    /// <summary>
    /// Gets the state of the active self defenses.
    /// </summary>
    ConcurrentDictionary<(Player Attacker, Player Defender), DateTime> SelfDefenseState { get; }

    /// <summary>
    /// Gets the initialized maps which are hosted on this context.
    /// </summary>
    ValueTask<IEnumerable<GameMap>> GetMapsAsync();

    /// <summary>
    /// Gets the players.
    /// </summary>
    ValueTask<IList<Player>> GetPlayersAsync();

    /// <summary>
    /// Adds the player to the game.
    /// </summary>
    /// <param name="player">The player.</param>
    ValueTask AddPlayerAsync(Player player);

    /// <summary>
    /// Gets the maps which is meant to be hosted by the game.
    /// </summary>
    /// <param name="mapId">The map identifier.</param>
    /// <param name="createIfNotExists">If set to <c>true</c>, the map is created if it doesn't exist yet.</param>
    /// <returns>
    /// The hosted GameMap instance.
    /// </returns>
    ValueTask<GameMap?> GetMapAsync(ushort mapId, bool createIfNotExists = true);

    /// <summary>
    /// Gets the mini game map which is meant to be hosted by the game.
    /// </summary>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <param name="requester">The requesting player.</param>
    /// <returns>
    /// The state of the mini game which contains the hosted GameMap instance.
    /// </returns>
    ValueTask<MiniGameContext> GetMiniGameAsync(MiniGameDefinition miniGameDefinition, Player requester);

    /// <summary>
    /// Removes the mini game instance from the context.
    /// </summary>
    /// <param name="miniGameContext">The context of the mini game.</param>
    ValueTask RemoveMiniGameAsync(MiniGameContext miniGameContext);

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
    ValueTask SendGlobalMessageAsync(string message, MessageType messageType);

    /// <summary>
    /// Sends a global chat message to all players of the game with the specified message type.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    ValueTask SendGlobalChatMessageAsync(string sender, string message, ChatMessageType messageType);

    /// <summary>
    /// Sends a golden global notification to all players of the game.
    /// </summary>
    /// <param name="message">The message.</param>
    ValueTask SendGlobalNotificationAsync(string message);

    /// <summary>
    /// Executes an action for each player.
    /// </summary>
    /// <param name="action">The action which is executed.</param>
    /// <remarks>
    /// Please avoid doing actions which may lead to the connected-state of the players.
    /// </remarks>
    ValueTask ForEachPlayerAsync(Func<Player, Task> action);
}