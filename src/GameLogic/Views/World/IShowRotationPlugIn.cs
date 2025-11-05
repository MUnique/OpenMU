// <copyright file="IShowRotationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about a rotation change of an object.
/// </summary>
public interface IShowRotationPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the rotation change of an object.
    /// </summary>
    /// <param name="rotatedObject">The rotated object.</param>
    ValueTask ShowRotationAsync(IRotatable rotatedObject);
}
