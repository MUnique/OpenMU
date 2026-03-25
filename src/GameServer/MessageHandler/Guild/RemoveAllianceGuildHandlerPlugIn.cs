// <copyright file="RemoveAllianceGuildHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using RemoveAllianceGuildRequest = MUnique.OpenMU.Network.Packets.ClientToServer.RemoveAllianceGuildRequest;

/// <summary>
/// Handler for remove alliance guild request packets (C1 EB 01).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RemoveAllianceGuildHandlerPlugIn_Name), Description = nameof(PlugInResources.RemoveAllianceGuildHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D5E6F7A8-B9C0-4D1E-2F3A-4B5C6D7E8F9A")]
[BelongsToGroup(AllianceGroupHandlerPlugIn.GroupKey)]
internal class RemoveAllianceGuildHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => RemoveAllianceGuildRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext)
        {
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            return;
        }

        RemoveAllianceGuildRequest request = packet;
        var targetGuildName = request.GuildName;

        if (string.IsNullOrWhiteSpace(targetGuildName))
        {
            return;
        }

        // Check that this guild is the alliance master
        var isMaster = await serverContext.GuildServer.IsAllianceMasterAsync(guildStatus.GuildId).ConfigureAwait(false);
        if (!isMaster)
        {
            return;
        }

        // Find the target guild by name
        var targetGuildId = await serverContext.GuildServer.GetGuildIdByNameAsync(targetGuildName).ConfigureAwait(false);
        if (targetGuildId == 0)
        {
            return;
        }

        // TODO: Maybe return GuildRelationshipChangeResult from the guild server to be more specific about the failure reason
        var success = await serverContext.GuildServer.RemoveAllianceGuildAsync(guildStatus.GuildId, targetGuildId).ConfigureAwait(false)
            ? GuildRelationshipChangeResultType.Success
            : GuildRelationshipChangeResultType.Failed;

        await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(GuildRelationshipType.Alliance, GuildRelationshipRequestType.Leave, success, player.GetId(player))).ConfigureAwait(false);
    }
}
