// <copyright file="RemoveDuplicateStatAttributesPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update removes stat attributes which are defined more than once for a character class.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("9C3E5B71-8D04-42A6-BF19-7A2D6C8E5B03")]
public class RemoveDuplicateStatAttributesPlugInSeason6 : RemoveDuplicateStatAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RemoveDuplicateStatAttributesSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;
}
