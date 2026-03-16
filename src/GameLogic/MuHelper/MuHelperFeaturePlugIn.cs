// <copyright file="MuHelperFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuHelper;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Feature plugin which provides the configuration for the reset feature.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.MuHelperFeaturePlugIn_Name), Description = nameof(PlugInResources.MuHelperFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E90A72C3-0459-4323-B6D3-171F88D35542")]
public class MuHelperFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<MuHelperConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc/>
    public MuHelperConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new MuHelperConfiguration();
}