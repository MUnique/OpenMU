﻿// <copyright file="QuestMonsterKillCountPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin increases the monster kill count of the quest state of active quests.
/// </summary>
[PlugIn("Count killed quest monsters", "This plugin increases the monster kill count of the quest state of active quests.")]
[Guid("416C2231-D7FE-414A-9321-26622E262EF5")]
public class QuestMonsterKillCountPlugIn : IAttackableGotKilledPlugIn
{
    /// <summary>
    /// Is called when an <see cref="IAttackable" /> object got killed by another.
    /// </summary>
    /// <param name="killed">The killed <see cref="IAttackable" />.</param>
    /// <param name="killer">The killer.</param>
    public void AttackableGotKilled(IAttackable killed, IAttacker killer)
    {
        if (!(killer is Player player && killed is Monster monster)
            || player.SelectedCharacter?.QuestStates is null)
        {
            return;
        }

        foreach (var questState in player.SelectedCharacter.QuestStates)
        {
            if (questState.ActiveQuest is null)
            {
                continue;
            }

            foreach (var killRequirement in questState.ActiveQuest.RequiredMonsterKills.Where(r => r.Monster == monster.Definition))
            {
                if (questState.RequirementStates.FirstOrDefault(s => s.Requirement == killRequirement)
                    is not { } requirementState)
                {
                    requirementState = player.PersistenceContext.CreateNew<QuestMonsterKillRequirementState>();
                    requirementState.Requirement = killRequirement;
                    questState.RequirementStates.Add(requirementState);
                }

                requirementState!.KillCount++;
            }
        }
    }
}