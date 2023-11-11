// <copyright file="IAssignable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Interface for a class which can assign values from another instance to the
/// own instance.
/// </summary>
/// <typeparam name="T">The type of the assignable.</typeparam>
public interface IAssignable<in T>
    where T : class
{
    /// <summary>
    /// Assigns the values of the other object to this instance.
    /// </summary>
    /// <param name="other">The other object.</param>
    /// <param name="gameConfiguration">The game configuration in which this instance lives in. The other object may come from another configuration.</param>
    void AssignValuesOf(T other, GameConfiguration gameConfiguration);
}

/// <summary>
/// Interface for a class which can assign values from another instance to the
/// own instance.
/// </summary>
public interface IAssignable : IAssignable<object>
{
}