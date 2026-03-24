// <copyright file="ShowGuildRelationshipChangeResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IGuildRelationshipChangeResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowGuildRelationshipChangeResultPlugIn_Name), Description = nameof(PlugInResources.ShowGuildRelationshipChangeResultPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E6F7A8B9-C0D1-4E2F-3A4B-5C6D7E8F9A0B")]
public class ShowGuildRelationshipChangeResultPlugIn : IGuildRelationshipChangeResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildRelationshipChangeResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildRelationshipChangeResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResultAsync(GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, bool success)
    {
        await this._player.Connection.SendGuildRelationshipChangeResultAsync(
            (GuildRelationshipRequest.GuildRelationshipType)(byte)relationshipType,
            (GuildRelationshipRequest.GuildRequestType)(byte)requestType,
            success).ConfigureAwait(false);
    }
}
