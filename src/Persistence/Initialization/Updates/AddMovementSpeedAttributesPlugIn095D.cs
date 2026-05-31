// <copyright file="AddMovementSpeedAttributesPlugIn095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds movement speed attributes to 0.95d game configurations.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("7C38C30F-163B-4625-A82D-5C3A0A9ED883")]
public class AddMovementSpeedAttributesPlugIn095D : AddMovementSpeedAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddMovementSpeedAttributes095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    protected override int MaximumItemLevel => Version095d.Items.Constants.MaximumItemLevel;
}
