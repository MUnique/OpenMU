// <copyright file="CastleSiegeGateListHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests for the Castle Siege gate list.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeGateListHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeGateListHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("904BE911-B46C-4C7A-BBC2-C5B845592C65")]
[BelongsToGroup(CastleSiegeNpcGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeGateListHandlerPlugIn : CastleSiegeNpcListHandlerBase
{
    /// <inheritdoc />
    public override byte Key => CastleSiegeGateListRequest.SubCode;

    /// <inheritdoc />
    protected override short MonsterNumber => CastleSiegeGate.MonsterNumber;
}
