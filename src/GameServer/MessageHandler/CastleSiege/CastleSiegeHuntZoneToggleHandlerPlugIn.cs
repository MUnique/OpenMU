// <copyright file="CastleSiegeHuntZoneToggleHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles public Land of Trials access-setting requests.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeHuntZoneToggleHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeHuntZoneToggleHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A97C3953-BE03-4C03-AB5A-C4FED16A629E")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeHuntZoneToggleHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private const short SeniorNumber = 223;
    private readonly CastleSiegeHuntZoneToggleAction _action = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => CastleSiegeHuntingZoneEntranceSetting.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < CastleSiegeHuntingZoneEntranceSetting.Length
            || player.OpenedNpc?.Definition.Number != SeniorNumber)
        {
            return;
        }

        var request = new CastleSiegeHuntingZoneEntranceSetting(packet);
        _ = await this._action.SetPublicAccessAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.IsPublic)
            .ConfigureAwait(false);
    }
}
