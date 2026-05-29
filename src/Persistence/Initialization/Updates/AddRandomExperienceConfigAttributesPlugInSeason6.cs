// <copyright file="AddRandomExperienceConfigAttributesPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the random experience config attributes for season 6.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("D1DC70A2-2614-4CC0-81C0-6C8253781019")]
public class AddRandomExperienceConfigAttributesPlugInSeason6 : AddRandomExperienceConfigAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddRandomExperienceConfigAttributesSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
