// <copyright file="RequestedItemConsumptionFailedExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IRequestedItemConsumptionFailedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(RequestedItemConsumptionFailedExtendedPlugIn), "The extended implementation of the IRequestedItemConsumptionFailedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("8F98AAF4-E329-4DE2-B8D5-9169B64E20B2")]
[MinimumClient(106, 3, ClientLanguage.English)]
public class RequestedItemConsumptionFailedExtendedPlugIn : IRequestedItemConsumptionFailedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestedItemConsumptionFailedExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public RequestedItemConsumptionFailedExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    /// <remarks>The server sends the current health/shield to the client, with <see cref="ItemConsumptionFailed"/>.</remarks>
    public async ValueTask RequestedItemConsumptionFailedAsync()
    {
        if (this._player.Attributes is null)
        {
            return;
        }

        await this._player.Connection.SendItemConsumptionFailedExtendedAsync(
                (uint)Math.Max(this._player.Attributes[Stats.CurrentHealth], 0f),
                (uint)Math.Max(this._player.Attributes[Stats.CurrentShield], 0f))
            .ConfigureAwait(false);
    }
}