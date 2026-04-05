// <copyright file="GuildRelationshipChangeResponseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;
using ViewGuildRelationshipType = MUnique.OpenMU.GameLogic.Views.Guild.GuildRelationshipType;
using ViewGuildRelationshipRequestType = MUnique.OpenMU.GameLogic.Views.Guild.GuildRelationshipRequestType;

/// <summary>
/// Handler for guild relationship change response packets (C1 E6) — the target guild master's answer.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GuildRelationshipChangeResponseHandlerPlugIn_Name), Description = nameof(PlugInResources.GuildRelationshipChangeResponseHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A2B3C4D5-E6F7-4A8B-9C0D-1E2F3A4B5C6D")]
internal class GuildRelationshipChangeResponseHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly GuildRelationshipChangeAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GuildRelationshipChangeResponse.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        GuildRelationshipChangeResponse response = packet;
        await this._action.ProcessResponseAsync(
            player,
            (ViewGuildRelationshipType)(byte)response.RelationshipType,
            (ViewGuildRelationshipRequestType)(byte)response.RequestType,
            response.Response).ConfigureAwait(false);
    }
}
