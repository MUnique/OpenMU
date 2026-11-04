// <copyright file="CastleSiegeRegisteredGuildListHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests for the current Castle Siege guild registrations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeRegisteredGuildListHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeRegisteredGuildListHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("1AA08B91-FD60-41EE-A927-C3A82793221D")]
internal class CastleSiegeRegisteredGuildListHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeRegisteredGuildsListRequest.Code;

    /// <inheritdoc/>
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var registrations = CastleSiegeHandlerContext.Get(player)?.RegisteredGuilds.Values
            .OrderBy(registration => registration.RegistrationOrder)
            .ToList()
            ?? [];
        return player.InvokeViewPlugInAsync<ICastleSiegeRegisteredGuildListPlugIn>(
            plugIn => plugIn.ShowRegisteredGuildListAsync(registrations));
    }
}
