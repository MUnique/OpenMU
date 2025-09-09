// <copyright file="ISummonConfigurationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Provides monster definitions for summon skills and allows to adjust their stats.
/// </summary>
[Guid("C1E5C063-9CF8-4FE7-9C0F-4BB6387E3C27")]
[PlugInPoint("Summon configuration", "Provides monster definitions for summon skills.")]
public interface ISummonConfigurationPlugIn : IStrategyPlugIn<short>
{
    /// <summary>
    /// Creates the monster definition which should be used for the summon skill.
    /// </summary>
    /// <param name="player">The summoning player.</param>
    /// <param name="skill">The used summon skill.</param>
    /// <param name="defaultDefinition">The default monster definition which would be used without this plug-in.</param>
    /// <returns>The monster definition which should be summoned, or <c>null</c> if the default mapping should be used.</returns>
    MonsterDefinition? CreateSummonMonsterDefinition(Player player, Skill skill, MonsterDefinition? defaultDefinition);
}
