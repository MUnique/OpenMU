// <copyright file="LegacyQuestRewardPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration.Quests;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ILegacyQuestRewardPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("Quest - Reward", "The default implementation of the ILegacyQuestRewardPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("53BEDA2A-9FE6-406D-AE6D-8E4DDDA3D73D")]
    public class LegacyQuestRewardPlugIn : ILegacyQuestRewardPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyQuestRewardPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public LegacyQuestRewardPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc />
        public void Show(Player receiver, QuestRewardType reward, int value)
        {
            var receiverId = receiver.GetId(this.player);
            switch (reward)
            {
                case QuestRewardType.LevelUpPoints:
                    this.player.Connection.SendLegacyQuestReward(receiverId, LegacyQuestReward.QuestRewardType.LevelUpPoints, (byte)value);
                    break;
                case QuestRewardType.CharacterEvolutionFirstToSecond:
                    this.player.Connection.SendLegacyQuestReward(receiverId, LegacyQuestReward.QuestRewardType.CharacterEvolutionFirstToSecond, (byte)(player.SelectedCharacter.CharacterClass.Number << 3));
                    var points = this.player.Level - 150;
                    if (points > 0)
                    {
                        this.player.Connection.SendLegacyQuestReward(receiverId, LegacyQuestReward.QuestRewardType.LevelUpPointsByNewClass, 1);
                    }

                    break;
                case QuestRewardType.CharacterEvolutionSecondToThird:
                    this.player.Connection.SendLegacyQuestReward(receiverId, LegacyQuestReward.QuestRewardType.CharacterEvolutionSecondToThird, (byte)(player.SelectedCharacter.CharacterClass.Number << 3));
                    break;
                case QuestRewardType.Attribute:
                    // TODO: only if character got the combo skill
                    this.player.Connection.SendLegacyQuestReward(receiverId, LegacyQuestReward.QuestRewardType.ComboSkill, 0);
                    break;
            }
        }
    }
}