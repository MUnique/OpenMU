// <copyright file="IRequestedItemConsumptionFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about the failed item consumption which was requested by the client.
    /// </summary>
    public interface IRequestedItemConsumptionFailedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows that the requested item consumption failed.
        /// </summary>
        void RequestedItemConsumptionFailed();
    }
}