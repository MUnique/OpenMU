// <copyright file="FixDuelArenaSafezoneMapUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This Sets the safezone of duel arena to lorencia.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("27714BB3-43F9-4D90-920F-98EF0EC20232")]
public class FixDuelArenaSafezoneMapUpdate : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Duel Arena Safezone Map";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Sets the safezone of duel arena to lorencia.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDuelArenaSafezoneMap;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 10, 28, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var duelArena = gameConfiguration.Maps.First(x => x.Number == DuelArena.Number);
        duelArena.SafezoneMap = gameConfiguration.Maps.First(m => m.Number == Lorencia.Number);
    }
}