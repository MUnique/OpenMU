// <copyright file="SetIntroducedGameVersionsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets in which version of the original game the maps, character classes, monsters,
/// items and mini games of the 0.75 data were introduced.
/// </summary>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Name), Description = nameof(Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("E2E08E19-CA90-47CB-8133-109E143EB501")]
public class SetIntroducedGameVersionsPlugIn075 : SetIntroducedGameVersionsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
