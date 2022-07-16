// <copyright file="QuestStateListLegacyRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the quest state list request for the legacy quest system (0xA0 identifier).
/// </summary>
[PlugIn("Quest state list request (legacy)", "Packet handler for the quest state list request for the legacy quest system (0xA0 identifier)")]
[Guid("C886C499-2DF4-48CE-BE8D-C1D6484C7C3D")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class QuestStateListLegacyRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc />
    public byte Key => LegacyQuestStateRequest.Code;

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var questState = player.SelectedCharacter?.QuestStates?.FirstOrDefault(state => state.Group == QuestConstants.LegacyQuestGroup);
        await player.InvokeViewPlugInAsync<IQuestStateResponsePlugIn>(p => p.ShowQuestStateAsync(questState)).ConfigureAwait(false);
    }
}