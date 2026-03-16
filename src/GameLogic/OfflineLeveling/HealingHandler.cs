// <copyright file="HealingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Handles health recovery for the offline leveling player and their party.
/// </summary>
public sealed class HealingHandler
{
    private static readonly ItemConsumeAction ConsumeAction = new();

    private static readonly ItemIdentifier[] HealthPotionPriority =
    [
        ItemConstants.LargeHealingPotion,
        ItemConstants.MediumHealingPotion,
        ItemConstants.SmallHealingPotion,
        ItemConstants.Apple,
    ];

    private readonly OfflineLevelingPlayer _player;
    private readonly IMuHelperSettings? _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealingHandler"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    /// <param name="config">The MU helper settings.</param>
    public HealingHandler(OfflineLevelingPlayer player, IMuHelperSettings? config)
    {
        this._player = player;
        this._config = config;
    }

    /// <summary>
    /// Performs health recovery actions for the player and their party.
    /// </summary>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask PerformHealthRecoveryAsync()
    {
        if (this._config is null || this._player.Attributes is null)
        {
            return;
        }

        await this.PerformSelfHealingAsync().ConfigureAwait(false);

        await this.PerformPartyHealingAsync().ConfigureAwait(false);
    }

    private async ValueTask PerformSelfHealingAsync()
    {
        if (this.IsHealthBelowThreshold(this._player, this._config!.HealThresholdPercent))
        {
            var healSkill = this.FindSkillByType(SkillType.Regeneration);
            if (healSkill is not null && this._config.AutoHeal)
            {
                await this._player.ApplyRegenerationAsync(this._player, healSkill).ConfigureAwait(false);
                return;
            }
        }

        if (this._config!.UseHealPotion && this.IsHealthBelowThreshold(this._player, this._config.PotionThresholdPercent))
        {
            await this.UseHealthPotionAsync().ConfigureAwait(false);
        }
    }

    private async ValueTask PerformPartyHealingAsync()
    {
        if (!this._config!.AutoHealParty || this._player.Party is not { } party)
        {
            return;
        }

        var healSkill = this.FindSkillByType(SkillType.Regeneration);
        if (healSkill is null)
        {
            return;
        }

        foreach (var member in party.PartyList.OfType<Player>())
        {
            if (member == this._player)
            {
                continue;
            }

            if (!member.IsActive() || !this._player.IsInRange(member, 8))
            {
                continue;
            }

            if (this.IsHealthBelowThreshold(member, this._config.HealPartyThresholdPercent))
            {
                await this._player.ApplyRegenerationAsync(member, healSkill).ConfigureAwait(false);
            }
        }
    }

    private bool IsHealthBelowThreshold(IAttackable target, int thresholdPercent)
    {
        if (target.Attributes is not { } attributes)
        {
            return false;
        }

        double hp = attributes[Stats.CurrentHealth];
        double maxHp = attributes[Stats.MaximumHealth];
        return maxHp > 0 && (hp * 100.0 / maxHp) <= thresholdPercent;
    }

    private async ValueTask UseHealthPotionAsync()
    {
        if (this._player.Inventory is null)
        {
            return;
        }

        foreach (var identifier in HealthPotionPriority)
        {
            var potion = this._player.Inventory.Items
                .FirstOrDefault(i => i.Definition?.Group == identifier.Group
                                     && i.Definition.Number == identifier.Number);
            if (potion is not null)
            {
                await ConsumeAction.HandleConsumeRequestAsync(
                    this._player, potion.ItemSlot, potion.ItemSlot, FruitUsage.Undefined).ConfigureAwait(false);
                return;
            }
        }
    }

    private SkillEntry? FindSkillByType(SkillType type)
    {
        return this._player.SkillList?.Skills.FirstOrDefault(s => s.Skill?.SkillType == type);
    }
}
