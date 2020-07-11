namespace MUnique.OpenMU.GameServer.RemoteView.Quest
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IQuestStepInfoPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("Quest - Step Info", "The default implementation of the IQuestStepInfoPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("099AACF3-2933-486C-83DE-01A340DACAA6")]
    public class QuestStepInfoPlugIn : IQuestStepInfoPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestStepInfoPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public QuestStepInfoPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc/>
        public void ShowQuestStepInfo(short questGroup, short stepNumber)
        {
            this.player.Connection.SendQuestStepInfo((ushort)stepNumber, (ushort)questGroup);
        }
    }
}