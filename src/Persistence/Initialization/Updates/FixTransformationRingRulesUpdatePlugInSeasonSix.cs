// <copyright file="FixTransformationRingRulesUpdatePlugInSeasonSix.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the transformation ring rules in season 6 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.FixTransformationRingRulesUpdatePlugIn_Name), Description = nameof(PlugInResources.FixTransformationRingRulesUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8064B574-07BD-4EA0-91D2-AB6A2BA1020B")]
public class FixTransformationRingRulesUpdatePlugInSeasonSix : FixTransformationRingRulesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
