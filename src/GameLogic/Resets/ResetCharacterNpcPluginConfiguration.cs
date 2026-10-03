// <copyright file="ResetCharacterNpcPluginConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Configuration for the <see cref="ResetCharacterNpcPlugin"/>.
/// </summary>
public class ResetCharacterNpcPluginConfiguration
{
    /// <summary>
    /// The default number of the NPC which resets the character ('Leo the Helper').
    /// </summary>
    internal const short DefaultResetNpcNumber = 371;

    /// <summary>
    /// Gets or sets the number of the NPC which resets the character ('Leo the Helper').
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ResetCharacterNpcPluginConfiguration_ResetNpcNumber_Name))]
    public short ResetNpcNumber { get; set; } = DefaultResetNpcNumber;
}
