// <copyright file="UpdateWingAndPetHitTollsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates wing and pet hit toll configuration for version 0.75.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.UpdateWingAndPetHitTollsPlugIn_Name), Description = nameof(PlugInResources.UpdateWingAndPetHitTollsPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6C3AA59D-83A9-45E7-A07F-B5CC77D7F528")]
public class UpdateWingAndPetHitTollsPlugIn075 : UpdateWingAndPetHitTollsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}