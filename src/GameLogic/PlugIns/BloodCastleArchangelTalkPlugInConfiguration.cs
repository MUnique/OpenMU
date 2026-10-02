// <copyright file="BloodCastleArchangelTalkPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="BloodCastleArchangelTalkPlugIn"/>.
/// </summary>
public class BloodCastleArchangelTalkPlugInConfiguration
{
    /// <summary>
    /// The default number of the Archangel NPC.
    /// </summary>
    internal const short DefaultArchangelNumber = 232;

    /// <summary>
    /// Gets or sets the number of the Archangel NPC, which accepts the quest item of the blood castle event.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BloodCastleArchangelTalkPlugInConfiguration_ArchangelNumber_Name))]
    public short ArchangelNumber { get; set; } = DefaultArchangelNumber;
}
