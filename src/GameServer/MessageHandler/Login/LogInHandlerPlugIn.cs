// <copyright file="LogInHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Network.Xor;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for login packets.
/// </summary>
[PlugIn("Login", "Packet handler for login packets.")]
[Guid("4A816FE5-809B-4D42-AF9F-1929FABD3295")]
[BelongsToGroup(LogInOutGroup.GroupKey)]
public class LogInHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly ISpanDecryptor _decryptor = new Xor3Decryptor(0);

    private readonly LoginAction _loginAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => LoginLongPassword.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < 42)
        {
            if (packet.Length > 28 + 3)
            {
                LoginShortPassword message = packet;
                await this.HandleLoginAsync(player, this.Decrypt(message.Username), this.Decrypt(message.Password), message.TickCount, ClientVersionResolver.Resolve(message.ClientVersion)).ConfigureAwait(false);
            }
            else
            {
                // we have some version like 0.75 which just uses three bytes as version identifier
                Login075 message = packet;
                await this.HandleLoginAsync(player, this.Decrypt(message.Username), this.Decrypt(message.Password), message.TickCount, ClientVersionResolver.Resolve(message.ClientVersion)).ConfigureAwait(false);
            }
        }
        else
        {
            LoginLongPassword message = packet;
            await this.HandleLoginAsync(player, this.Decrypt(message.Username), this.Decrypt(message.Password), message.TickCount, ClientVersionResolver.Resolve(message.ClientVersion)).ConfigureAwait(false);
        }
    }

    private string Decrypt(Span<byte> stringSpan)
    {
        this._decryptor.Decrypt(stringSpan);
        return stringSpan.ExtractString(0, stringSpan.Length, Encoding.UTF8);
    }

    private async ValueTask HandleLoginAsync(Player player, string username, string password, uint tickCount, ClientVersion version)
    {
        if (player.Logger.IsEnabled(LogLevel.Debug))
        {
            player.Logger.LogDebug($"User tries to log in. username:{username}, version:{version}, tickCount:{tickCount} ");
        }

        await this._loginAction.LoginAsync(player, username, password).ConfigureAwait(false);
        if (player is RemotePlayer remotePlayer)
        {
            // Set Version in RemotePlayer so that the right plugins will be selected
            remotePlayer.ClientVersion = version;
        }
    }
}