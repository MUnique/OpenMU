﻿// <copyright file="Exile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Exile map.
/// </summary>
internal class Exile : Initialization.BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Exile"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Exile(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 5;

    /// <inheritdoc/>
    protected override string MapName => "Exile";
}