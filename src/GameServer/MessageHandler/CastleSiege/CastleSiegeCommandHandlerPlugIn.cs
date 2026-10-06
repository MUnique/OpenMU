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

        // Team is the client's command-group (squad) slot index, not an audience/authorization field - the
        // audience is already determined by which players receive the resulting packet. The client stores it
        // into a fixed GuildCommander[7] buffer with no bounds check on its side, so an out-of-range value here
        // would be an out-of-bounds write on every recipient's client. Reject rather than clamp or relay blindly.
        if (request.Team > 6)
        {
            return;
        }

        // Reject unmapped command bytes instead of defaulting to Wait: the client's rendering path leaves
        // width/height uninitialized for any value above Wait (2), so a malformed byte should never reach it.
        var command = request.Command switch
        {
            CastleSiegeGuildCommandType.Attack => CastleSiegeCommandType.Attack,
            CastleSiegeGuildCommandType.Defend => CastleSiegeCommandType.Defend,
            CastleSiegeGuildCommandType.Wait => CastleSiegeCommandType.Wait,
            _ => (CastleSiegeCommandType?)null,
        };

        if (command is not { } validCommand)
        {
            return;
        }

        await CastleSiegeGuildCommandAction.IssueCommandAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.Team,
                request.PositionX,
                request.PositionY,
                validCommand)
            .ConfigureAwait(false);
    }
}
