// <copyright file="GuildRoleAssignHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameServer.RemoteView.Guild;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for guild role assign packets.
/// </summary>
/// <remarks>
/// Only known <see cref="GuildRoleAssignType"/> values are processed, anything else is ignored.
/// The role limits are always enforced server-side for every known request type, because
/// the type byte is client-controlled input. No dedicated
/// server response packet exists; on success the guild server publishes the change which updates
/// the member's guild status and views.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.GuildRoleAssignHandlerPlugIn_Name), Description = nameof(PlugInResources.GuildRoleAssignHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("76191DD2-AFC3-4FFF-8CB0-BB8DD1641B15")]
internal class GuildRoleAssignHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly GuildRoleAssignAction _roleAssignAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GuildRoleAssignRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < GuildRoleAssignRequest.Length)
        {
            return;
        }

        GuildRoleAssignRequest request = packet;
        if (!Enum.IsDefined(typeof(GuildRoleAssignType), request.Type))
        {
            return;
        }

        var position = request.Role.ConvertToPosition();

        if (position is null)
        {
            player.Logger.LogWarning("Rejected guild role assignment: invalid role {Role} for target {TargetName}, could be hack attempt.", request.Role, request.PlayerName);
            return;
        }

        await this._roleAssignAction.AssignRoleAsync(player, request.PlayerName, position.Value).ConfigureAwait(false);
    }
}
