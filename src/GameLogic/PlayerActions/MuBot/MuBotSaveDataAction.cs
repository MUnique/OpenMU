namespace MUnique.OpenMU.GameLogic.PlayerActions.MuBot
{
    using System;
    using MUnique.OpenMU.GameLogic.Views.MuBot;

    /// <summary>
    /// Action to update mu bot status
    /// </summary>
    public class MuBotSaveDataAction
    {
        /// <summary>
        /// Toggle mu bot status
        /// </summary>
        /// <param name="player">the player.</param>
        /// <param name="data">mu bot data to be saved.</param>
        public void MuBotSaveData(Player player, Span<byte> data)
        {
            player.ViewPlugIns.GetPlugIn<IMuBotSaveDataResponse>()?.SendMuBotSavedDataResponse(data);
        }
    }
}