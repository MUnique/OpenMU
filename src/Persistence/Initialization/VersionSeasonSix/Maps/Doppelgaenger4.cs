// <copyright file="Doppelgaenger4.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Map initialization for the Doppelgaenger 4 (a.k.a. "Double Gear", "Double Goer", etc.) event map.
/// </summary>
internal class Doppelgaenger4 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 68;

    /// <summary>
    /// Initializes a new instance of the <see cref="Doppelgaenger4"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Doppelgaenger4(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    internal static LocalizedString Name => LocalizedString.FromResource(() => MapNames.Doppelgaenger4);

    /// <inheritdoc />
    protected override byte MapNumber => Number;

    /// <inheritdoc />
    protected override LocalizedString MapName => Name;

    /// <summary>
    /// Gets the safezone map number. Players who die inside the doppelganger event map
    /// respawn at Elvenland.
    /// </summary>
    protected override byte SafezoneMapNumber => Elvenland.Number;
}