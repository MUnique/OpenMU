// <copyright file="AttackSpeedSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Defines which speed attribute scales the delay between two attacks.
/// </summary>
public enum AttackSpeedSource
{
    /// <summary>
    /// The physical attack speed.
    /// </summary>
    [Display(Name = nameof(Properties.PlugInResources.AttackSpeedSource_AttackSpeed), ResourceType = typeof(Properties.PlugInResources))]
    AttackSpeed = 0,

    /// <summary>
    /// The magic speed.
    /// </summary>
    [Display(Name = nameof(Properties.PlugInResources.AttackSpeedSource_MagicSpeed), ResourceType = typeof(Properties.PlugInResources))]
    MagicSpeed = 1,
}
