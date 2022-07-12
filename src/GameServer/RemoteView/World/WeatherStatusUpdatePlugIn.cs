// <copyright file="WeatherStatusUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IWeatherStatusUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("Weather status update", "The default implementation of the IWeatherStatusUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("44369927-4EE4-47D7-9C6C-DD74FC824071")]
public class WeatherStatusUpdatePlugIn : IWeatherStatusUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherStatusUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public WeatherStatusUpdatePlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public ValueTask ShowWeatherAsync(byte weather, byte variation)
    {
        return this._player.Connection.SendWeatherStatusUpdateAsync(weather, variation);
    }
}