// <copyright file="ShowCastleSiegeRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCastleSiegeRegistrationResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCastleSiegeRegistrationResultPlugIn), "The default implementation of the IShowCastleSiegeRegistrationResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("D4E5F678-9012-23AB-CDEF-456789012ABC")]
public class ShowCastleSiegeRegistrationResultPlugIn : IShowCastleSiegeRegistrationResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCastleSiegeRegistrationResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCastleSiegeRegistrationResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResultAsync(CastleSiegeRegistrationResult result)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // TODO: Implement proper packet sending when server-to-client castle siege packets are defined
        var message = result switch
        {
            CastleSiegeRegistrationResult.Success => "Successfully registered for castle siege",
            CastleSiegeRegistrationResult.Unregistered => "Successfully unregistered from castle siege",
            CastleSiegeRegistrationResult.NotInGuild => "You are not in a guild",
            CastleSiegeRegistrationResult.NotTheGuildMaster => "Only the guild master can register",
            CastleSiegeRegistrationResult.NotInAlliance => "You must be in an alliance to register",
            CastleSiegeRegistrationResult.NotAllianceMaster => "Only the alliance master can register for castle siege",
            CastleSiegeRegistrationResult.RegistrationClosed => "Castle siege registration is closed",
            CastleSiegeRegistrationResult.AlreadyRegistered => "Your alliance is already registered",
            CastleSiegeRegistrationResult.NotRegistered => "Your alliance is not registered",
            _ => "Castle siege registration completed"
        };

        await connection.SendServerMessageAsync(
            ServerMessage.MessageType.GoldenCenter,
            message).ConfigureAwait(false);
    }
}
