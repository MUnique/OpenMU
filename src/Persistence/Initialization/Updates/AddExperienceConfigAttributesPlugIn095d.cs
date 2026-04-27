// <copyright file="AddExperienceConfigAttributesPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the experience config attributes for version 0.95d.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("9A166583-C3E7-4E04-924C-F01FF9840974")]
public class AddExperienceConfigAttributesPlugIn095d : AddExperienceConfigAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddExperienceConfigAttributes095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
