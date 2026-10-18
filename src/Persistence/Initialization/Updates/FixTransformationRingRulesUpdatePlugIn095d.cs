// <copyright file="FixTransformationRingRulesUpdatePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the transformation ring rules in version 0.95d configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.FixTransformationRingRulesUpdatePlugIn_Name), Description = nameof(PlugInResources.FixTransformationRingRulesUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A7451A17-98C9-4915-A21C-344ABA2F02C8")]
public class FixTransformationRingRulesUpdatePlugIn095D : FixTransformationRingRulesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
