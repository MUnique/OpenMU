// <copyright file="CastleSiegeRegistrationStateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles Castle Siege registration-state requests.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeRegistrationStateHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeRegistrationStateHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("94E8D09A-7DBC-41B0-8A85-CA31F032B0FF")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal class CastleSiegeRegistrationStateHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CastleSiegeRegistrationStateAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeRegistrationStateRequest.SubCode;

    /// <inheritdoc/>
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        return this._action.ShowStateAsync(player, CastleSiegeHandlerContext.Get(player));
    }
}
