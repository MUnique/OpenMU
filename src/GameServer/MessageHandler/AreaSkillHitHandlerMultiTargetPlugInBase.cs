// <copyright file="AreaSkillHitHandlerMultiTargetPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Base implementation for a skill hit plugin where the message contains multiple target ids.
/// </summary>
internal class AreaSkillHitHandlerMultiTargetPlugInBase
{
    private readonly AreaSkillHitAction _skillHitAction = new();

    /// <summary>
    /// Tries the get skill entry.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skillIndex">Index of the skill.</param>
    /// <param name="skillEntry">The skill entry.</param>
    /// <returns><see langword="true"/>, if the skill entry was found; otherwise, <see langword="false"/>.</returns>
    protected bool TryGetSkillEntry(Player player, byte skillIndex, [NotNullWhen(true)] out SkillEntry? skillEntry)
    {
        if (player.SkillList is null
            || player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.GetSkillByIndex(skillIndex) is not { } skill
            || player.SkillList.GetSkill((ushort)skill.Number) is not { } entry)
        {
            skillEntry = null;
            return false;
        }

        skillEntry = entry;
        return true;
    }

    /// <summary>
    /// Handles the hits with the specified skill index.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skillEntry">Entry of the skill.</param>
    /// <param name="targetId">The target id.</param>
    protected async ValueTask AttackTargetAsync(Player player, SkillEntry skillEntry, ushort targetId)
    {
        if (player.GetObject(targetId) is IAttackable target)
        {
            if (target is IObservable observable
                && observable.Observers.Contains(player))
            {
                await this._skillHitAction.AttackTargetAsync(player, target, skillEntry).ConfigureAwait(false);
            }
            else
            {
                // Client may be out of sync (or it's an hacker attempt),
                // so we tell him the object is out of scope - this should prevent further attempts to attack it.
                await player.InvokeViewPlugInAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(target.GetAsEnumerable())).ConfigureAwait(false);
            }
        }
    }
}