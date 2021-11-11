// <copyright file="SilentMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Silent map.
/// </summary>
internal class SilentMap : BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SilentMap"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SilentMap(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 40;

    /// <inheritdoc/>
    protected override string MapName => "Silent Map?";
}