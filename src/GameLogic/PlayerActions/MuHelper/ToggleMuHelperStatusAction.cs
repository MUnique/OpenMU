namespace MUnique.OpenMU.GameLogic.PlayerActions.MuHelper
{
    using MUnique.OpenMU.GameLogic.Views.MuHelper;

    /// <summary>
    /// Action to update mu helper status
    /// </summary>
    public class ToggleMuHelperStatusAction
    {
        /// <summary>
        /// Toggle mu helper status
        /// </summary>
        /// <param name="player">the player.</param>
        /// <param name="desiredStatus">status to be set.</param>
        public void ToggleMuHelperStatus(Player player, byte desiredStatus)
        {
            // todo: do validations before turn on
            player.ViewPlugIns.GetPlugIn<IToggleMuHelperStatus>()?.ToggleMuHelperStatus(desiredStatus);
        }
    }
}