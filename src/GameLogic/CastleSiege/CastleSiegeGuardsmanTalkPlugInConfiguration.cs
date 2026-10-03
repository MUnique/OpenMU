// <copyright file="CastleSiegeGuardsmanTalkPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="CastleSiegeGuardsmanTalkPlugIn"/>.
/// </summary>
public class CastleSiegeGuardsmanTalkPlugInConfiguration
{
    /// <summary>
    /// The default number of the guardsman NPC, which opens the Land of Trials entry dialog.
    /// </summary>
    internal const short DefaultGuardsmanNumber = 224;

    /// <summary>
    /// Gets or sets the number of the guardsman NPC, which opens the Land of Trials entry dialog.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.CastleSiegeGuardsmanTalkPlugInConfiguration_GuardsmanNumber_Name))]
    public short GuardsmanNumber { get; set; } = DefaultGuardsmanNumber;
}
