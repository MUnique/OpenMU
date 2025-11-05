// <copyright file="GuildRelationshipChangeResponseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for guild relationship change response packets (alliance or hostility).
/// </summary>
[PlugIn(nameof(GuildRelationshipChangeResponseHandlerPlugIn), "Handler for guild relationship change response packets.")]
[Guid("7B3C4D5E-2A1F-4E6B-8C9D-F0E1A2B3C4D5")]
internal class GuildRelationshipChangeResponseHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly AllianceResponseAction _allianceResponseAction = new();
    private readonly HostilityResponseAction _hostilityResponseAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GuildRelationshipChangeResponse.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        GuildRelationshipChangeResponse response = packet;

        if (response.RelationshipType == GuildRelationshipType.Alliance && response.RequestType == GuildRequestType.Join)
        {
            await this._allianceResponseAction.RespondToAllianceAsync(player, response.Response).ConfigureAwait(false);
        }
        else if (response.RelationshipType == GuildRelationshipType.Hostility && response.RequestType == GuildRequestType.Join)
        {
            await this._hostilityResponseAction.RespondToHostilityAsync(player, response.Response).ConfigureAwait(false);
        }
    }
}
