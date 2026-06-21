// <copyright file="AddMovementSpeedAttributesPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds movement speed attributes to season 6 game configurations.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("1D4968DA-9C9C-42A7-AF80-D4811535EC63")]
public class AddMovementSpeedAttributesPlugInSeason6 : AddMovementSpeedAttributesPlugInBase
{
    private const int SeasonSixMaximumItemLevel = 15;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddMovementSpeedAttributesSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    protected override int MaximumItemLevel => SeasonSixMaximumItemLevel;
}
