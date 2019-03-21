// <copyright file="ICloseVaultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about a closed vault.
    /// </summary>
    public interface ICloseVaultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Closes the vault.
        /// </summary>
        void CloseVault();
    }
}