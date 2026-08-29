// <copyright file="CastleSiegeTaxInfoHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles Castle Siege tax information requests.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeTaxInfoHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeTaxInfoHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("190A4CCF-F15E-483D-886A-7E960E7750D6")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeTaxInfoHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private const short SeniorNumber = 223;
    private readonly CastleSiegeTaxInfoAction _action = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => CastleSiegeTaxInfoRequest.SubCode;

    /// <inheritdoc />
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        return packet.Length >= CastleSiegeTaxInfoRequest.Length
               && player.OpenedNpc?.Definition.Number == SeniorNumber
            ? this._action.ShowAsync(player, CastleSiegeHandlerContext.Get(player))
            : ValueTask.CompletedTask;
    }
}
