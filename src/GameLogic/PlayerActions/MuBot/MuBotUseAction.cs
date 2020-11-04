namespace MUnique.OpenMU.GameLogic.PlayerActions.MuBot
{
    using MUnique.OpenMU.GameLogic.Views.MuBot;

    /// <summary>
    /// Action to send back to the client the mu bot data
    /// </summary>
    public class MuBotUseAction
    {
        /// <summary>
        /// send mu bot data back to the client
        /// </summary>
        /// <param name="player">the player.</param>
        /// <param name="desiredStatus">status to be set.</param>
        public void UseMuBot(Player player, byte desiredStatus)
        {
            var status = desiredStatus;

            // check min level to use
            // check money
            player.ViewPlugIns.GetPlugIn<IMuBotUseResponse>()?.SendMuBotUseResponse(status);
        }
    }
}