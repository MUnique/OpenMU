// <copyright file="IObserverDependentNpcTypeNumber.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Provides observer-dependent flags which are encoded into an NPC's type number when it enters scope.
/// </summary>
/// <remarks>
/// This contract is consumed by the extended NPC scope view. Legacy views intentionally keep their original encoding.
/// </remarks>
public interface IObserverDependentNpcTypeNumber
{
    /// <summary>
    /// Gets the type-number flags for the specified observer.
    /// </summary>
    /// <param name="observer">The player receiving the NPC scope update.</param>
    /// <returns>The flags to combine with the NPC's base type number.</returns>
    ushort GetTypeNumberFlags(Player observer);
}
