// <copyright file="UpdateWingAndPetHitTollsPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates wing and pet hit toll configuration for version 0.95d.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.UpdateWingAndPetHitTollsPlugIn_Name), Description = nameof(PlugInResources.UpdateWingAndPetHitTollsPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8455AD08-2FE4-468B-901A-F7006D099FEC")]
public class UpdateWingAndPetHitTollsPlugIn095d : UpdateWingAndPetHitTollsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}