// <copyright file="CharacterGroupHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Packet handler for character packets (0xF3 identifier).
    /// </summary>
    internal class CharacterGroupHandler : BasePacketHandler, IPacketHandler
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CharacterGroupHandler));

        private readonly CreateCharacterAction createCharacterAction;

        private readonly DeleteCharacterAction deleteCharacterAction;

        private readonly RequestCharacterListAction requestCharacterListAction;

        private readonly FocusCharacterAction focusCharacterAction;

        private readonly SelectCharacterAction characterSelectAction;

        private readonly IncreaseStatsAction increaseStatsAction;

        private readonly SaveKeyConfigurationAction saveKeyConfigurationAction;

        private readonly AddMasterPointAction addMasterPointAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterGroupHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public CharacterGroupHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.createCharacterAction = new CreateCharacterAction(gameContext);
            this.deleteCharacterAction = new DeleteCharacterAction();
            this.requestCharacterListAction = new RequestCharacterListAction();
            this.focusCharacterAction = new FocusCharacterAction();
            this.characterSelectAction = new SelectCharacterAction();
            this.increaseStatsAction = new IncreaseStatsAction();
            this.saveKeyConfigurationAction = new SaveKeyConfigurationAction();
            this.addMasterPointAction = new AddMasterPointAction(gameContext);
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
        {
            switch (packet[3])
            {
                case 0:
                    this.requestCharacterListAction.RequestCharacterList(player);
                    break;
                case 1:
                    this.ReadCreateCharacter(player, packet);
                    break;
                case 2:
                    this.ReadDeleteCharacter(player, packet);
                    break;
                case 0x15:
                    this.ReadFocusCharacter(player, packet);
                    break;
                case 3:
                    this.ReadSelectCharacter(player, packet);
                    break;
                case 6:
                    this.ReadIncreaseStats(player, packet);
                    break;
                case 0x12: ////Data Loaded by Client
                    player.ClientReadyAfterMapChange();
                    break;
                case 0x30: ////GCSkillKeyRecv
                    this.saveKeyConfigurationAction.SaveKeyConfiguration(player, packet.Slice(4).ToArray());
                    break;
                case 0x52:
                    this.AddMasterSkillPoint(player, packet);
                    break;
                default:
                    Log.Warn($"Packet F3 {packet[3] : X} isn't implemented.");
                    break;
            }
        }

        private void AddMasterSkillPoint(Player player, Span<byte> packet)
        {
            // LO HI
            // C1 08 F3 52 A6 01 00 00
            var skillId = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
            this.addMasterPointAction.AddMasterPoint(player, skillId);
        }

        private void ReadIncreaseStats(Player player, Span<byte> buffer)
        {
            var statType = (CharacterStatType)buffer[4];
            this.increaseStatsAction.IncreaseStats(player, statType.GetAttributeDefinition());
        }

        private void ReadFocusCharacter(Player player, Span<byte> packet)
        {
            string characterName = packet.ExtractString(4, 10, Encoding.UTF8);
            this.focusCharacterAction.FocusCharacter(player, characterName);
        }

        private void ReadSelectCharacter(Player player, Span<byte> packet)
        {
            string characterName = packet.ExtractString(4, 10, Encoding.UTF8);
            this.characterSelectAction.SelectCharacter(player, characterName);
        }

        private void ReadDeleteCharacter(Player player, Span<byte> packet)
        {
            string characterName = packet.ExtractString(4, 10, Encoding.UTF8);
            string securityCode = packet.ExtractString(14, 7, Encoding.UTF8);
            this.deleteCharacterAction.DeleteCharacter(player, characterName, securityCode);
        }

        private void ReadCreateCharacter(Player player, Span<byte> packet)
        {
            var characterName = packet.ExtractString(4, 10, Encoding.UTF8);
            int classNumber = packet[14] >> 2;
            this.createCharacterAction.CreateCharacter(player, characterName, classNumber);
        }
    }
}
