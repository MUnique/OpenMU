// <copyright file="BaseMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Base class for a map initializer which provides some common basic functionality.
/// </summary>
internal abstract class BaseMapInitializer : Initialization.BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMapInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected BaseMapInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the version prefix for Terrain resources.
    /// </summary>
    protected override string TerrainVersionPrefix => "075_";
}