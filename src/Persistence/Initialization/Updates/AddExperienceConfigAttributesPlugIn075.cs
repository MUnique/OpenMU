// <copyright file="AddExperienceConfigAttributesPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the experience config attributes for version 0.75.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("5F412933-CC0F-483B-B6AE-7B358A6257FD")]
public class AddExperienceConfigAttributesPlugIn075 : AddExperienceConfigAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddExperienceConfigAttributes075;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
