// <copyright file="IShowVaultLockChangeResponse.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Vault;

/// <summary>
/// Interface of a view whose implementation shows the result of a request to enter, modify or remove the pin code of the vault.
/// </summary>
public interface IShowVaultLockChangeResponse : IViewPlugIn
{
    /// <summary>
    /// Shows the result of a request to enter, modify or remove the pin code of the vault.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowResponseAsync(VaultLockChangeResult result);
}