// <copyright file="ITargetedSkillPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when skill is casted on a target.
/// The key is the skill number.
/// </summary>
[Guid("744cd623-1811-46ee-abb2-2e9c016f0102")]
[PlugInPoint("Targeted skill plugins", "Is called when a target got attacked by a targeted skill.")]
public interface ITargetedSkillPlugin : IStrategyPlugIn<short>
{
    /// <summary>
    /// Callback function to trigger when specific skill is invoked.
    /// <param name="player">The player invoking the skill.</param>
    /// <param name="target">The target of the skill.</param>
    /// <param name="skillId">The skill identifier.</param>
    /// <returns>Returns the coroutine for the skill action.</returns>
    /// </summary>
    ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId);
}
