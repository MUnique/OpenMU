// <copyright file="IUpdateVaultStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Vault;

/// <summary>
/// Interface of a view whose implementation updates the vault state on client side.
/// </summary>
public interface IUpdateVaultStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the vault state on client side.
    /// </summary>
    ValueTask UpdateStateAsync();
}