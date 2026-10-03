// <copyright file="DuelArena.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Map initialization for the duel arena map.
/// </summary>
internal class DuelArena : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 64;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelArena"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DuelArena(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    internal static LocalizedString Name => LocalizedString.FromResource(() => MapNames.DuelArena);

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override LocalizedString MapName => Name;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Lorencia.Number;
}