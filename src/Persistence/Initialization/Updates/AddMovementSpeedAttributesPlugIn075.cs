// <copyright file="AddMovementSpeedAttributesPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds movement speed attributes to 0.75 game configurations.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("890E2FCB-EC93-4CC1-84FC-67A1B398D5C8")]
public class AddMovementSpeedAttributesPlugIn075 : AddMovementSpeedAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddMovementSpeedAttributes075;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    protected override int MaximumItemLevel => Version075.Items.Constants.MaximumItemLevel;
}
