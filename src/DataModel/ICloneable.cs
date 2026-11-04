// <copyright file="ICloneable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Interface for a class which can be cloned to a new instance based on the given
/// <see cref="GameConfiguration"/>.
/// </summary>
/// <typeparam name="T">The type of the assignable.</typeparam>
public interface ICloneable<out T>
    where T : class
{
    /// <summary>
    /// Clones this instance, base on the specified game configuration.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The cloned instance.</returns>
    T Clone(GameConfiguration gameConfiguration);
}