// <copyright file="CastleSiegeTaxChangeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;
using TaxType = MUnique.OpenMU.GameLogic.CastleSiege.CastleSiegeTaxType;

/// <summary>
/// Handles Castle Siege tax-rate change requests.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeTaxChangeHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeTaxChangeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("52F0F2F3-643A-433A-9A1A-7F70B827E18D")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeTaxChangeHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private const short SeniorNumber = 223;
    private readonly CastleSiegeTaxRateChangeAction _action = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => CastleSiegeTaxChangeRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < CastleSiegeTaxChangeRequest.Length
            || player.OpenedNpc?.Definition.Number != SeniorNumber)
        {
            return;
        }

        var request = new CastleSiegeTaxChangeRequest(packet);
        _ = await this._action.ChangeAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                (TaxType)request.TaxType,
                request.TaxValue)
            .ConfigureAwait(false);
    }
}
