// <copyright file="LegacyQuestStateDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ILegacyQuestStateDialogPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("Quest - Status Dialog", "The default implementation of the ILegacyQuestStateDialogPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("F43DA7A9-1ACC-4FBB-9E15-8BE977F4CAF9")]
public class LegacyQuestStateDialogPlugIn : ILegacyQuestStateDialogPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyQuestStateDialogPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public LegacyQuestStateDialogPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowAsync()
    {
        if (this._player.Connection is not { } connection || this._player.SelectedCharacter is null)
        {
            return;
        }

        var questState = this._player.SelectedCharacter.QuestStates.FirstOrDefault(s => s.Group == QuestConstants.LegacyQuestGroup);
        var quest = questState?.ActiveQuest ?? this._player.GetNextLegacyQuest();
        await connection.SendLegacyQuestStateDialogAsync((byte)(quest?.Number ?? 0), this._player.GetLegacyQuestStateByte()).ConfigureAwait(false);

        if (quest?.RequiredMonsterKills.Any() ?? false)
        {
            int Write()
            {
                var size = LegacyQuestMonsterKillInfoRef.Length;
                var span = connection.Output.GetSpan(size)[..size];
                var packet = new LegacyQuestMonsterKillInfoRef(span);
                packet.QuestIndex = (byte)quest.Number;
                int i = 0;
                foreach (var requirement in quest.RequiredMonsterKills)
                {
                    var monsterState = packet[i];
                    monsterState.MonsterNumber = (uint)requirement.Monster!.Number;
                    monsterState.KillCount = (uint)(questState?.RequirementStates.FirstOrDefault(r => r.Requirement == requirement)?.KillCount ?? 0);
                    i++;
                }

                return size;
            }

            await connection.SendAsync(Write).ConfigureAwait(false);
        }
    }
}