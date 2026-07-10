// <copyright file="SpeedHackDetectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A feature plugin that provides configuration and acts as a trigger control for speedhack anti-cheat checks.
/// </summary>
[PlugIn]
[Display(Name = "Speedhack Anti-Cheat", Description = "Detects and prevents player walk and attack speedhacking.")]
[Guid("A95A8D2F-A0C3-442E-995C-005B5C1B42D2")]
public class SpeedHackDetectPlugIn : IFeaturePlugIn, ISupportCustomConfiguration<SpeedHackDetectConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc/>
    public SpeedHackDetectConfiguration? Configuration { get; set; }

    /// <inheritdoc/>
    public object CreateDefaultConfig() => new SpeedHackDetectConfiguration();
}
