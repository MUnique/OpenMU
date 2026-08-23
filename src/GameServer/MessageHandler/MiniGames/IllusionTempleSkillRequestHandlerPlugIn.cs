// <copyright file="IllusionTempleSkillRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for illusion temple special skill request packets (210 to 213 - Order of Protection,
/// Restraint, Tracking and Weaken).
/// </summary>
/// <remarks>
/// The packet belongs to the 0xBF group, which is dispatched by the <see cref="MuHelperGroupHandler"/>.
/// Therefore this is a sub packet handler which is selected by the sub code, and not a handler of its own.
/// </remarks>
[PlugIn]
[Guid("3E9B2F7D-6C1A-4E3D-9A2E-5D8B1C7F0A6E")]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class IllusionTempleSkillRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => IllusionTempleSkillRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < IllusionTempleSkillRequest.Length
            || player.CurrentMiniGame is not IllusionTempleContext illusionTemple)
        {
            return;
        }

        IllusionTempleSkillRequest request = packet;
        await illusionTemple.UseSkillAsync(player, request.SkillNumber, request.TargetObjectIndex).ConfigureAwait(false);
    }
}
