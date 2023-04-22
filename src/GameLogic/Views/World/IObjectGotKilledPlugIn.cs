// <copyright file="IObjectGotKilledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about killed objects.
/// </summary>
public interface IObjectGotKilledPlugIn : IViewPlugIn
{
    /// <summary>
    /// An object got killed by another object.
    /// </summary>
    /// <param name="killedObject">The killed object.</param>
    /// <param name="killerObject">The object which killed the object.</param>
    ValueTask ObjectGotKilledAsync(IAttackable killedObject, IAttacker? killerObject);
}