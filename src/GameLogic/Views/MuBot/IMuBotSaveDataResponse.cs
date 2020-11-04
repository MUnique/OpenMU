namespace MUnique.OpenMU.GameLogic.Views.MuBot
{
    using System;

    /// <summary>
    /// Interface of a view whose implementation sends back to the client mu bot data.
    /// </summary>
    public interface IMuBotSaveDataResponse : IViewPlugIn
    {
        /// <summary>
        /// send the mu bot saved data
        /// </summary>
        /// <param name="data">The data status.</param>
        void SendMuBotSavedDataResponse(Span<byte> data);
    }
}