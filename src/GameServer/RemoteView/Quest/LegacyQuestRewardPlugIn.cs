// <copyright file="LegacyQuestRewardPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ILegacyQuestRewardPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("Quest - Reward", "The default implementation of the ILegacyQuestRewardPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("53BEDA2A-9FE6-406D-AE6D-8E4DDDA3D73D")]
public class LegacyQuestRewardPlugIn : ILegacyQuestRewardPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyQuestRewardPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public LegacyQuestRewardPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowAsync(Player receiver, QuestRewardType reward, int value, AttributeDefinition? attributeReward)
    {
        var receiverId = receiver.GetId(this._player);
        switch (reward)
        {
            case QuestRewardType.LevelUpPoints:
                await this._player.Connection.SendLegacyQuestRewardAsync(receiverId, LegacyQuestReward.QuestRewardType.LevelUpPoints, (byte)value).ConfigureAwait(false);
                break;
            case QuestRewardType.CharacterEvolutionFirstToSecond:
                await this._player.Connection.SendLegacyQuestRewardAsync(receiverId, LegacyQuestReward.QuestRewardType.CharacterEvolutionFirstToSecond, (byte)(this._player.SelectedCharacter!.CharacterClass!.Number << 3)).ConfigureAwait(false);
                break;
            case QuestRewardType.CharacterEvolutionSecondToThird:
                await this._player.Connection.SendLegacyQuestRewardAsync(receiverId, LegacyQuestReward.QuestRewardType.CharacterEvolutionSecondToThird, (byte)(this._player.SelectedCharacter!.CharacterClass!.Number << 3)).ConfigureAwait(false);
                break;
            case QuestRewardType.Attribute:
                if (attributeReward == Stats.IsSkillComboAvailable)
                {
                    await this._player.Connection.SendLegacyQuestRewardAsync(receiverId, LegacyQuestReward.QuestRewardType.ComboSkill, 0).ConfigureAwait(false);
                }

                if (attributeReward == Stats.PointsPerLevelUp)
                {
                    var questLevel = this._player.GetQuestState(QuestConstants.LegacyQuestGroup)?.ActiveQuest?.MinimumCharacterLevel ?? 220;
                    var points = (this._player.Level - questLevel) * value;
                    await this._player.Connection.SendLegacyQuestRewardAsync(receiverId, LegacyQuestReward.QuestRewardType.LevelUpPointsPerLevelIncrease, (byte)points).ConfigureAwait(false);
                }

                break;
            default:
                this._player.Logger.LogDebug("Unknown quest reward: {0}", reward);
                break;
        }
    }
}