// <copyright file="IShowRageAttackPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view plugin whose implementation informs about rage attacks.
/// </summary>
public interface IShowRageAttackPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the attack.
    /// </summary>
    /// <param name="attacker">The attacking object.</param>
    /// <param name="target">The targeted object.</param>
    /// <param name="skillId">The skill identifier.</param>
    ValueTask ShowAttackAsync(IIdentifiable attacker, IIdentifiable? target, ushort skillId);
}