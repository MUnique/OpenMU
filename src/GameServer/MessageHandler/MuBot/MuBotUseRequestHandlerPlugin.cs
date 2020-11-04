namespace MUnique.OpenMU.GameServer.MessageHandler.MuBot
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.MuBot;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for Mu Bot Use Request Handler
    /// </summary>
    [PlugIn("MuBotUseRequestHandlerPlugin", "Handler for mu bot use request.")]
    [Guid("26d0fef9-8171-4098-87ea-030054163511")]
    [BelongsToGroup(MuBotGroupHandler.GroupKey)]
    public class MuBotUseRequestHandlerPlugin : ISubPacketHandlerPlugIn
    {
        private readonly MuBotUseAction muBotUseAction = new MuBotUseAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => MuBotUseRequest.SubCode;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            MuBotUseRequest message = packet;
            this.muBotUseAction.UseMuBot(player, message.Status);
        }
    }
}