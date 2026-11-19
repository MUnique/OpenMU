// <copyright file="LoginWithTicketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.LoginTickets;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the login with a ticket instead of a password, see <see cref="LoginWithTicket"/>.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.LoginWithTicketHandlerPlugIn_Name), Description = nameof(PlugInResources.LoginWithTicketHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("AB27DAC7-F0D8-4B59-86F8-FCB819F0C4EC")]
[BelongsToGroup(LogInOutGroup.GroupKey)]
public class LoginWithTicketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly LoginAction _loginAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => LoginWithTicket.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < LoginWithTicket.Length)
        {
            return;
        }

        LoginWithTicket message = packet;
        var ticketService = new LoginTicketService(() => player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration));
        await this._loginAction.LoginWithTicketAsync(player, message.Token, ticketService).ConfigureAwait(false);
        if (player is RemotePlayer remotePlayer)
        {
            // Set Version in RemotePlayer so that the right plugins will be selected
            remotePlayer.ClientVersion = ClientVersionResolver.Resolve(message.ClientVersion);
        }
    }
}
