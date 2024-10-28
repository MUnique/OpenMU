// <copyright file="GuildMasterAnswerHandlerPlugIn.cs" company="MUnique">
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
[PlugIn("GuildMasterAnswerHandlerPlugIn", "Handler for guild master answer packets.")]
[Guid("3715c03e-9c77-4e43-9f6b-c1db3a2c3233")]
internal class GuildMasterAnswerHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly GuildMasterAnswerAction _answerAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GuildMasterAnswer.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        GuildMasterAnswer answer = packet;
        await this._answerAction.ProcessAnswerAsync(player, answer.ShowCreationDialog ? GuildMasterAnswerAction.Answer.ShowDialog : GuildMasterAnswerAction.Answer.Cancel).ConfigureAwait(false);
    }
}