// <copyright file="IUpdateVaultMoneyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about a change in the stored money in the vault.
    /// </summary>
    public interface IUpdateVaultMoneyPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Refresh vault and player money on client side.
        /// </summary>
        void UpdateVaultMoney();
    }
}