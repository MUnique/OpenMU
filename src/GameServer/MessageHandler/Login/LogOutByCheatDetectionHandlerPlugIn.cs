// <copyright file="LogOutByCheatDetectionHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for logout by cheat packets.
/// </summary>
[PlugIn("Logout by cheat handler", "Handler for logout (by cheat) packets.")]
[Guid("AE611B1E-3E3D-4189-B39C-79696D27BFBD")]
[BelongsToGroup(LogInOutGroup.GroupKey)]
public class LogOutByCheatDetectionHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly LogoutAction _logoutAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => LogOutByCheatDetection.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        player.Logger.LogError("Logged out by detected cheat on client side. Player: {player}", player);
        await this._logoutAction.LogoutAsync(player, LogoutType.CloseGame).ConfigureAwait(false);
    }
}