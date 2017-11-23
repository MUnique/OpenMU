// <copyright file="TalkNpcHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for talk npc request packets.
    /// </summary>
    internal class TalkNpcHandler : BasePacketHandler
    {
        private readonly TalkNpcAction talkNpcAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TalkNpcHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public TalkNpcHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.talkNpcAction = new TalkNpcAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, byte[] packet)
        {
            ushort id = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            if (player.CurrentMap.GetObject(id) is NonPlayerCharacter npc)
            {
                this.talkNpcAction.TalkToNpc(player, npc);
            }
        }
    }
}
