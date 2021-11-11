﻿// <copyright file="DuelArena.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Map initialization for the duel arena map.
/// </summary>
internal class DuelArena : BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuelArena"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DuelArena(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 64;

    /// <inheritdoc/>
    protected override string MapName => "Duel Arena";
}