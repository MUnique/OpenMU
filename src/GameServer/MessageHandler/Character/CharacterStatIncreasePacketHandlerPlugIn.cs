// <copyright file="CharacterStatIncreasePacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for character stat increase packets (0xF3, 0x06 identifier).
/// </summary>
[PlugIn("Character - Stat increase ", "Packet handler for character stat increase packets (0xF3, 0x06 identifier).")]
[Guid("5DC06689-B2DD-4CA2-8F93-97FB1198BA70")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class CharacterStatIncreasePacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly IncreaseStatsAction _increaseStatsAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => 6;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        IncreaseCharacterStatPoint message = packet;
        await this._increaseStatsAction.IncreaseStatsAsync(player, message.StatType.GetAttributeDefinition()).ConfigureAwait(false);
    }
}