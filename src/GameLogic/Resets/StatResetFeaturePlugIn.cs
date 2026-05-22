// <copyright file="StatResetFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Feature plugin which provides the configuration for the stat reset feature.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.StatResetFeaturePlugIn_Name), Description = nameof(PlugInResources.StatResetFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("0F1E2D3C-4B5A-6978-8C7D-6E5F4A3B2C1D")]
public class StatResetFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<StatResetConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <inheritdoc/>
    public StatResetConfiguration? Configuration { get; set; }

    /// <inheritdoc/>
    public object CreateDefaultConfig() => new StatResetConfiguration();
}
