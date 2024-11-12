// <copyright file="CancelGuildCreationHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for guild master answer packets.
/// </summary>
[PlugIn(nameof(CancelGuildCreationHandlerPlugIn), "Handler for CancelGuildCreation packets.")]
[Guid("CDB87F7F-24FC-42E1-B375-9EAAEAAC0F8C")]
internal class CancelGuildCreationHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly GuildMasterAnswerAction _answerAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CancelGuildCreation.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._answerAction.ProcessAnswerAsync(player, GuildMasterAnswerAction.Answer.Cancel).ConfigureAwait(false);
    }
}