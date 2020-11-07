namespace MUnique.OpenMU.GameLogic.MuBot
{
    using System;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.MuBot;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Toggle player Mu Bot
    /// </summary>
    public static class MuBotPlayerExtensions
    {
        /// <summary>
        /// Toggle mu bot of player
        /// </summary>
        /// <param name="player">the current player object.</param>
        /// <param name="status">status to set.</param>
        public static void ToggleMuBot(this Player player, MuBotStatus status)
        {
            player.ViewPlugIns.GetPlugIn<IMuBotUseResponse>()
                ?.SendMuBotUseResponse((byte)status, 0, 0);
        }

        /// <summary>
        /// Send message of money collect.
        /// </summary>
        /// <param name="player">the current player object.</param>
        /// <param name="money">amount of money.</param>
        public static void SendMuBotMoneyUsed(this Player player, int money)
        {
            player.ViewPlugIns.GetPlugIn<IMuBotUseResponse>()
                ?.SendMuBotUseResponse((byte)MuBotStatus.Enabled, (uint)money, 1);
        }

        /// <summary>
        /// Show message to player
        /// </summary>
        /// <param name="player">the current player object.</param>
        /// <param name="message">message to show.</param>
        public static void ShowMessage(this Player player, string message)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, MessageType.BlueNormal);
        }
    }
}