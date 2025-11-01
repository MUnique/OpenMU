// <copyright file="ShowAllianceResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAllianceResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowAllianceResponsePlugIn), "The default implementation of the IShowAllianceResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("B2C3D4E5-F678-9012-3456-7890ABCDEF01")]
public class ShowAllianceResponsePlugIn : IShowAllianceResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllianceResponsePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAllianceResponsePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResponseAsync(AllianceResponse response, string targetGuildName)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // TODO: Implement proper packet sending when server-to-client alliance packets are defined
        var message = response switch
        {
            AllianceResponse.Success => $"Alliance with {targetGuildName} successful",
            AllianceResponse.Failed => $"Alliance with {targetGuildName} failed",
            AllianceResponse.RequestSent => $"Alliance request sent to {targetGuildName}",
            AllianceResponse.NotInGuild => "You are not in a guild",
            AllianceResponse.NotTheGuildMaster => "Only the guild master can manage alliances",
            AllianceResponse.GuildNotFound => $"Guild {targetGuildName} not found",
            AllianceResponse.GuildMasterOffline => $"{targetGuildName} guild master is offline",
            AllianceResponse.HasHostility => "Cannot form alliance with hostile relationship",
            AllianceResponse.AllianceFull => "Alliance is full",
            AllianceResponse.AlreadyInAlliance => $"{targetGuildName} is already in an alliance",
            AllianceResponse.Removed => $"Removed from alliance by {targetGuildName}",
            _ => "Alliance operation completed"
        };

        await connection.SendServerMessageAsync(
            ServerMessage.MessageType.GoldenCenter,
            message).ConfigureAwait(false);
    }
}
