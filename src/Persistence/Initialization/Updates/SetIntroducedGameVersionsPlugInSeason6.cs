// <copyright file="SetIntroducedGameVersionsPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets in which version of the original game the maps, character classes, monsters,
/// items and mini games of the Season 6 data were introduced.
/// </summary>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Name), Description = nameof(Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("EB12AA19-24E5-40C3-8906-93115C74AFBB")]
public class SetIntroducedGameVersionsPlugInSeason6 : SetIntroducedGameVersionsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
