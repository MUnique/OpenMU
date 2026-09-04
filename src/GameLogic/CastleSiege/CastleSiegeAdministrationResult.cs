// <copyright file="CastleSiegeAdministrationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// The result of an administrative Castle Siege operation.
/// </summary>
/// <param name="IsSuccess">A value indicating whether the operation succeeded.</param>
/// <param name="Error">The error when the operation was not successful.</param>
public sealed record CastleSiegeAdministrationResult(bool IsSuccess, CastleSiegeAdministrationError Error)
{
    /// <summary>
    /// Gets a successful result.
    /// </summary>
    public static CastleSiegeAdministrationResult Succeeded { get; } = new(true, CastleSiegeAdministrationError.None);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The operation error.</param>
    /// <returns>The failed result.</returns>
    public static CastleSiegeAdministrationResult Failed(CastleSiegeAdministrationError error) => new(false, error);
}
