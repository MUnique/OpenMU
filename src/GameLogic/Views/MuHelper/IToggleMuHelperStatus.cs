namespace MUnique.OpenMU.GameLogic.Views.MuHelper
{
    /// <summary>
    /// Interface of a view whose implementation toggles the mu helper status.
    /// </summary>
    public interface IToggleMuHelperStatus : IViewPlugIn
    {
        /// <summary>
        /// Toggle Mu Helper Status
        /// </summary>
        /// <param name="status">The desired status.</param>
        void ToggleMuHelperStatus(byte status);
    }
}