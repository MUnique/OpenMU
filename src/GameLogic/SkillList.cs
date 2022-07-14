// <copyright file="SkillList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using Nito.AsyncEx.Synchronous;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// The implementation of the skill list, which automatically adds the passive skill power ups to the player.
/// </summary>
public class SkillList : ISkillList
{
    private readonly IDictionary<ushort, SkillEntry> _availableSkills;

    private readonly ICollection<SkillEntry> _learnedSkills;

    private readonly ICollection<SkillEntry> _itemSkills;

    private readonly Player _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillList"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public SkillList(Player player)
    {
        if (player.SelectedCharacter is null)
        {
            throw new ArgumentException("SelectedCharacter must be set.");
        }

        if (player.Inventory is null)
        {
            throw new ArgumentException("Inventory must be set.");
        }

        this._player = player;
        this._learnedSkills = this._player.SelectedCharacter.LearnedSkills ?? new List<SkillEntry>();
        this._learnedSkills.Where(entry => entry.Skill is null).ForEach(entry => throw Error.NotInitializedProperty(entry, nameof(entry.Skill)));

        this._availableSkills = this._learnedSkills.ToDictionary(skillEntry => skillEntry.Skill!.Number.ToUnsigned());
        this._itemSkills = new List<SkillEntry>();
        this._player.Inventory.EquippedItems
            .Where(item => item.HasSkill)
            .Where(item => (item.Definition ?? throw Error.NotInitializedProperty(item, nameof(item.Definition))).Skill != null)
            .ForEach(item => this.AddItemSkillAsync(item.Definition!.Skill!).AsTask().WaitAndUnwrapException());
        this._player.Inventory.EquippedItemsChanged += this.Inventory_WearingItemsChangedAsync;
        foreach (var skill in this._learnedSkills.Where(s => s.Skill!.SkillType == SkillType.PassiveBoost))
        {
            this.CreatePowerUpForPassiveSkill(skill);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SkillEntry> Skills => this._availableSkills.Values;

    /// <inheritdoc/>
    public byte SkillCount => (byte)this._availableSkills.Count;

    /// <inheritdoc/>
    public SkillEntry? GetSkill(ushort skillId)
    {
        this._availableSkills.TryGetValue(skillId, out var result);
        return result;
    }

    /// <inheritdoc/>
    public async ValueTask AddLearnedSkillAsync(Skill skill)
    {
        var skillEntry = this._player.PersistenceContext.CreateNew<SkillEntry>();
        skillEntry.Skill = skill;
        skillEntry.Level = 0;
        await this.AddLearnedSkillAsync(skillEntry);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> RemoveItemSkillAsync(ushort skillId)
    {
        this._availableSkills.TryGetValue(skillId, out var skillEntry);
        if (skillEntry is null)
        {
            return false;
        }

        // We need to take into account that we there might be multiple items equipped with the same skill
        var skillRemoved = this._itemSkills.Remove(skillEntry);
        if (skillRemoved && this._itemSkills.All(s => s.Skill!.Number != skillId))
        {
            await this._player.InvokeViewPlugInAsync<ISkillListViewPlugIn>(p => p.RemoveSkillAsync(skillEntry.Skill!)).ConfigureAwait(false);
            this._availableSkills.Remove(skillId);
        }

        return true;
    }

    /// <inheritdoc/>
    public bool ContainsSkill(ushort skillId)
    {
        return this._availableSkills.ContainsKey(skillId);
    }

    private async ValueTask AddItemSkillAsync(Skill skill)
    {
        var skillEntry = new SkillEntry
        {
            Skill = skill,
            Level = 0,
        };
        this._itemSkills.Add(skillEntry);

        // Item skills are always level 0, so it doesn't matter which one is added to the dictionary.
        if (!this.ContainsSkill((ushort)skill.Number))
        {
            this._availableSkills.Add(skill.Number.ToUnsigned(), skillEntry);
            await this._player.InvokeViewPlugInAsync<ISkillListViewPlugIn>(p => p.AddSkillAsync(skill)).ConfigureAwait(false);
        }
    }

    private async ValueTask AddLearnedSkillAsync(SkillEntry skill)
    {
        this._availableSkills.Add(skill.Skill!.Number.ToUnsigned(), skill);
        this._learnedSkills.Add(skill);

        await this._player.InvokeViewPlugInAsync<ISkillListViewPlugIn>(p => p.AddSkillAsync(skill.Skill)).ConfigureAwait(false);
        if (skill.Skill.SkillType == SkillType.PassiveBoost)
        {
            this.CreatePowerUpForPassiveSkill(skill);
        }
    }

    private void CreatePowerUpForPassiveSkill(SkillEntry skillEntry)
    {
        this.CreatePowerUpWrappers(skillEntry);
    }

    private void CreatePowerUpWrappers(SkillEntry skillEntry)
    {
        var masterDefinition = skillEntry.Skill!.MasterDefinition;
        if (masterDefinition is null)
        {
            return;
        }

        if (masterDefinition.TargetAttribute is null)
        {
            // log?
            return;
        }

        // maybe to do: We don't need to hold it, as it's added to the player attributes.
        _ = new PowerUpWrapper(new PassiveSkillBoostPowerUp(skillEntry), masterDefinition.TargetAttribute, this._player.Attributes!);
    }

    private async ValueTask Inventory_WearingItemsChangedAsync(ItemEventArgs eventArgs)
    {
        var item = eventArgs.Item;
        if (!item.HasSkill || item.Definition?.Skill is null)
        {
            return;
        }

        var inventory = this._player.Inventory;
        if (inventory!.EquippedItems.Contains(item))
        {
            await this.AddItemSkillAsync(item.Definition.Skill);
        }
        else
        {
            await this.RemoveItemSkillAsync(item.Definition.Skill.Number.ToUnsigned());
        }
    }

    private sealed class PassiveSkillBoostPowerUp : IElement
    {
        public PassiveSkillBoostPowerUp(SkillEntry skillEntry)
        {
            this.Value = skillEntry.CalculateValue();
            this.AggregateType = skillEntry.Skill!.MasterDefinition!.Aggregation;
            skillEntry.PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(SkillEntry.Level))
                {
                    this.Value = skillEntry.CalculateValue();
                    this.ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public event EventHandler? ValueChanged;

        public float Value { get; private set; }

        public AggregateType AggregateType { get; }
    }
}