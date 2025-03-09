// <copyright file="ResetFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Feature plugin which provides the configuration for the reset feature.
/// </summary>
[PlugIn("Reset Feature", "Provides configuration for the reset feature.")]
[Guid("6A9D585D-79D7-4674-B6EA-7E87392FA501")]
public class ResetFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<ResetConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <inheritdoc/>
    public ResetConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new ResetConfiguration();
}