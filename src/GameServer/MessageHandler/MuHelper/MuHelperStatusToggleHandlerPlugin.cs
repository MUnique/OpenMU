using MUnique.OpenMU.GameLogic.PlayerActions.MuHelper;

namespace MUnique.OpenMU.GameServer.MessageHandler.MuHelper
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for party kick packets.
    /// </summary>
    [PlugIn("Mu Helper Status-Toggle Handler Plugin", "Handler for mu helper status toggle.")]
    [Guid("26d0fef9-8171-4098-87ea-030054163511")]
    [BelongsToGroup(MuHelperGroupHandlerPlugin.GroupKey)]
    public class MuHelperStatusToggleHandlerPlugin : ISubPacketHandlerPlugIn
    {
        private readonly ToggleMuHelperStatusAction toggleMuHelperStatusAction = new ToggleMuHelperStatusAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => MuHelperStatusToggle.SubCode;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            MuHelperStatusToggle message = packet;
            this.toggleMuHelperStatusAction.ToggleMuHelperStatus(player, message.Status);
        }
    }
}