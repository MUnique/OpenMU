// <copyright file="CastleSiegeCommandHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles directional guild commands issued by Castle Siege alliance masters.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeCommandHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeCommandHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("DF41BA2B-8C50-4783-8332-8AC58ABF1FFD")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeCommandHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => CastleGuildCommand.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < CastleGuildCommand.Length)
        {
            return;
        }

        var request = new CastleGuildCommand(packet);

        // request.Team is intentionally ignored: the server re-derives the issuer's actual side to prevent
        // a spoofed Team value from mis-targeting the command (see CastleSiegeGuildCommandAction).
        var command = request.Command switch
        {
            CastleSiegeGuildCommandType.Attack => CastleSiegeCommandType.Attack,
            CastleSiegeGuildCommandType.Defend => CastleSiegeCommandType.Defend,
            _ => CastleSiegeCommandType.Wait,
        };

        await CastleSiegeGuildCommandAction.IssueCommandAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.PositionX,
                request.PositionY,
                command)
            .ConfigureAwait(false);
    }
}
