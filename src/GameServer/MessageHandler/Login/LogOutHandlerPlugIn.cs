// <copyright file="LogOutHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for logout packets.
/// </summary>
[PlugIn("Logout handler", "Handler for logout packets.")]
[Guid("84108668-70A0-42F6-AA80-B43757F12836")]
[BelongsToGroup(LogInOutGroup.GroupKey)]
public class LogOutHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly LogoutAction _logoutAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => LogOut.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        LogOut message = packet;
        await this._logoutAction.LogoutAsync(player, (LogoutType)message.Type).ConfigureAwait(false);
    }
}