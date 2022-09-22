// <copyright file="IShowRageAttackRangePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view plugin whose implementation informs about rage attacks.
/// </summary>
public interface IShowRageAttackRangePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the attack.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="targets">The target objects which will receive the hits.</param>
    ValueTask ShowRageAttackRangeAsync(ushort skillId, IEnumerable<IIdentifiable> targets);
}