// <copyright file="IObservableGameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Interface for an observable game server.
/// </summary>
public interface IObservableGameServer : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the identifier of the server.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Gets the maps which are hosted on this server.
    /// </summary>
    IList<IGameMapInfo> Maps { get; }

    /// <summary>
    /// Registers an observers to a game map.
    /// </summary>
    /// <param name="mapId">The id of the map.</param>
    /// <param name="worldObserver">The world observer.</param>
    void RegisterMapObserver(Guid mapId, ILocateable worldObserver);

    /// <summary>
    /// Unregisters the map observer.
    /// </summary>
    /// <param name="mapId">The map identifier.</param>
    /// <param name="worldObserverId">The world observer identifier.</param>
    void UnregisterMapObserver(Guid mapId, ushort worldObserverId);
}

/// <summary>
/// Information about a concrete instance of a game map.
/// </summary>
public interface IGameMapInfo : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the unique map identifier.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the map number.
    /// </summary>
    short MapNumber { get; }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    /// <value>
    /// The name of the map.
    /// </value>
    string MapName { get; }

    /// <summary>
    /// Gets the terrain data of the map.
    /// </summary>
    /// <value>
    /// The terrain data.
    /// </value>
    byte[]? TerrainData { get; }

    /// <summary>
    /// Gets the players which are currently playing on the map.
    /// </summary>
    IList<IPlayerInfo> Players { get; }

    /// <summary>
    /// Gets the player count.
    /// </summary>
    int PlayerCount { get; }
}

/// <summary>
/// Information about a player.
/// </summary>
public interface IPlayerInfo
{
    /// <summary>
    /// Gets the name of the character.
    /// </summary>
    string CharacterName { get; }

    /// <summary>
    /// Gets the name of the account.
    /// </summary>
    string AccountName { get; }

    /// <summary>
    /// Gets the x coordinate on the game map.
    /// </summary>
    byte LocationX { get; }

    /// <summary>
    /// Gets the y coordinate on the game map.
    /// </summary>
    byte LocationY { get; }
}
