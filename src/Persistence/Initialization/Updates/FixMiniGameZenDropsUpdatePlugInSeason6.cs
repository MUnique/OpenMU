// <copyright file="FixMiniGameZenDropsUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update configures the Devil Square and Illusion Temple maps of season 6 to drop no zen,
/// and the monsters of the Doppelganger, Imperial Guardian and Kanturu events which are only rewarded by their event.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.FixMiniGameZenDropsUpdatePlugIn_Name), Description = nameof(PlugInResources.FixMiniGameZenDropsUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("DB7D7563-0125-4DD8-89EA-0905BD49B5F2")]
public class FixMiniGameZenDropsUpdatePlugInSeason6 : FixMiniGameZenDropsUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    protected override IReadOnlyCollection<short> MapNumbersWithoutZen =>
    [
        Version095d.Maps.DevilSquare1.Number,
        DevilSquare5.Number,
        IllusionTemple1.Number,
        IllusionTemple2.Number,
        IllusionTemple3.Number,
        IllusionTemple4.Number,
        IllusionTemple5.Number,
        IllusionTemple6.Number,
    ];

    /// <inheritdoc />
    protected override IReadOnlyCollection<short> MonsterNumbersWithoutZen =>
    [
        .. KanturuEvent.MonsterNumbersWithoutZen,
        .. FortressOfImperialGuardian1.MonsterNumbersWithoutZen,
        .. DoppelgangerMonsters.MonsterNumbersWithoutZen,
    ];
}
