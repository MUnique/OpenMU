// <copyright file="MonsterStatsUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Sets the stats of the monsters of Season 6 to the Season 6 data.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MonsterStatsUpdatePlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MonsterStatsUpdatePlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("3CD69B94-3CE1-4F26-8BEC-73357D67BFD3")]
public class MonsterStatsUpdatePlugInSeason6 : MonsterStatsUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
