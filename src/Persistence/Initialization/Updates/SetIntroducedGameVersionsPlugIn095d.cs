// <copyright file="SetIntroducedGameVersionsPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets in which version of the original game the maps, character classes, monsters,
/// items and mini games of the 0.95d data were introduced.
/// </summary>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Name), Description = nameof(Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("C9820A53-5153-4A42-8047-D476DF81FDEA")]
public class SetIntroducedGameVersionsPlugIn095d : SetIntroducedGameVersionsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
