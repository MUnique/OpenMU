// <copyright file="NpcTalkPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for a <see cref="NpcTalkPlugInBase"/>.
/// </summary>
public class NpcTalkPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the NPC which is handled by the plugin.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.NpcTalkPlugInConfiguration_Npc_Name), Description = nameof(PlugInResources.NpcTalkPlugInConfiguration_Npc_Description))]
    public MonsterDefinition? Npc { get; set; }
}
