// <copyright file="IUpdateMoneyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about a changed inventory money value.
    /// </summary>
    public interface IUpdateMoneyPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the money value.
        /// </summary>
        void UpdateMoney();
    }
}