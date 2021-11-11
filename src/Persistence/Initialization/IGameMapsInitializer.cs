// <copyright file="IGameMapsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

/// <summary>
/// Initializer for game map data.
/// </summary>
public interface IGameMapsInitializer : IInitializer
{
    /// <summary>
    /// Sets the safezone maps.
    /// </summary>
    void SetSafezoneMaps();
}