namespace MUnique.OpenMU.GameLogic.Views.MuBot
{
    /// <summary>
    /// Interface of a view whose implementation toggles the mu bot status.
    /// </summary>
    public interface IMuBotUseResponse : IViewPlugIn
    {
        /// <summary>
        /// Toggle mu bot Status
        /// </summary>
        /// <param name="status">The desired status.</param>
        void SendMuBotUseResponse(byte status);
    }
}