// <copyright file="FixTransformationRingRulesUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the transformation ring rules in version 0.75 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.FixTransformationRingRulesUpdatePlugIn_Name), Description = nameof(PlugInResources.FixTransformationRingRulesUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))][Guid("0F9B05C4-E1DC-498E-9EFB-E314D520A010")]
public class FixTransformationRingRulesUpdatePlugIn075 : FixTransformationRingRulesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
