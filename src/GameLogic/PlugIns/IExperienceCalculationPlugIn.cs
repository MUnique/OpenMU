// <copyright file="IExperienceCalculationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when the experience for a kill is calculated, so that the
/// amount can be modified, e.g. by an event or a party bonus.
/// </summary>
/// <remarks>
/// The plugins are called after the configured experience rates and the map multiplier have been
/// applied, and before the random multiplier of <see cref="Attributes.Stats.RandomExperienceMinMultiplier"/>
/// and <see cref="Attributes.Stats.RandomExperienceMaxMultiplier"/> is applied.
/// Because the plugins are not called in a defined order, they should only apply factors or
/// summands which are independent of each other.
/// </remarks>
[Guid("35B0F1F6-EE79-4C0E-9C5C-6A9E0F8DCB70")]
[PlugInPoint("Experience calculation", "Plugins which modify the amount of experience which a player gains for a kill.")]
public interface IExperienceCalculationPlugIn
{
    /// <summary>
    /// Is called when the experience for a kill has been calculated.
    /// </summary>
    /// <param name="player">The player which gains the experience.</param>
    /// <param name="args">The arguments, which hold the calculated experience.</param>
    ValueTask CalculateExperienceAsync(Player player, ExperienceCalculationArgs args);
}
