// <copyright file="LearnablesConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for items (e.g. scrolls, orbs) which add a skill when being consumed.
/// </summary>
[Guid("FD86947E-0B94-4490-8158-63B11A61565F")]
[PlugIn]
[Display(Name = nameof(PlugInResources.LearnablesConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.LearnablesConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class LearnablesConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.AllScrolls;

    /// <inheritdoc/>
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        var skill = this.GetLearnableSkill(item, player.GameContext.Configuration);

        if (skill is null || player.SkillList!.ContainsSkill(skill.Number.ToUnsigned()))
        {
            return false;
        }

        if (!await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            return false;
        }

        await player.SkillList.AddLearnedSkillAsync(skill).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Besides the requirements of the item, the player has to comply with the requirements of the skill.
    /// Otherwise, the player could learn a skill which it can't use, and the item would be lost.
    /// E.g. the Orb of Summoning has no energy requirement itself, but the skills of its levels have.
    /// </remarks>
    protected override bool CheckPreconditions(Player player, Item item)
    {
        return base.CheckPreconditions(player, item)
               && player.CompliesRequirements(item)
               && (this.GetLearnableSkill(item, player.GameContext.Configuration) is not { } skill || player.CompliesRequirements(skill));
    }

    /// <summary>
    /// Gets the learnable skill.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The skill to learn.</returns>
    protected virtual Skill? GetLearnableSkill(Item item, GameConfiguration gameConfiguration)
    {
        return item.Definition?.Skill;
    }
}