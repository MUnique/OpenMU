// <copyright file="Doppelgaenger2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Map initialization for the Doppelgaenger 2 (a.k.a. "Double Gear", "Double Goer", etc.) event map.
/// </summary>
internal class Doppelgaenger2 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 66;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Doppelgaenger 2";

    /// <summary>
    /// Initializes a new instance of the <see cref="Doppelgaenger2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Doppelgaenger2(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override byte MapNumber => Number;

    /// <inheritdoc />
    protected override string MapName => Name;
}