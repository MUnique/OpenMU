// <copyright file="SkillList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// The implementation of the skill list, which automatically adds the passive skill power ups to the player.
/// </summary>
public sealed class SkillList : ISkillList, IDisposable
{
    private const ushort DurabilityReduction1SkillId = 300;
    private const ushort DurabilityReduction1FistMasterSkillId = 578;
    private const short TwistingSlashMasterySkillId = 332;
    private const short RagefulBlowMasterySkillId = 333;
    private const short TripleShotMasterySkillId = 418;
    private const short SleepStrengthenerSkillId = 454;
    private const short DrainLifeStrengthenerSkillId = 458;

    private readonly short[] _castedSkillsWithPassiveBoost = [
        TwistingSlashMasterySkillId,
        RagefulBlowMasterySkillId,
        TripleShotMasterySkillId,
        SleepStrengthenerSkillId,
        DrainLifeStrengthenerSkillId
    ];

    private readonly IDictionary<ushort, SkillEntry> _availableSkills;

    private readonly ICollection<SkillEntry> _learnedSkills;

    private readonly ICollection<SkillEntry> _itemSkills;

    private readonly Player _player;

    private List<IDisposable>? _passivePowerUps;

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

        this._availableSkills = this.CreateAvailableSkills();
        this._itemSkills = new List<SkillEntry>();
        this._player.Inventory.EquippedItems
            .Where(item => item.HasSkill)
            .Where(item => (item.Definition ?? throw Error.NotInitializedProperty(item, nameof(item.Definition))).Skill != null)
            .ForEach(item => this.AddItemSkillAsync(item.Definition!.Skill!).AsTask().WaitAndUnwrapException());
        this._player.Inventory.EquippedItemsChanged += this.Inventory_WearingItemsChangedAsync;
        foreach (var skill in this._learnedSkills
            .Where(s => s.Skill!.SkillType == SkillType.PassiveBoost || this._castedSkillsWithPassiveBoost.Contains(s.Skill.Number)))
        {
            this.CreatePowerUpForPassiveSkill(skill);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SkillEntry> Skills => this._availableSkills.Values;

    /// <inheritdoc/>
    public byte SkillCount => (byte)this._availableSkills.Count;

    private List<IDisposable> PassivePowerUps => this._passivePowerUps ??= new();

    /// <inheritdoc />
    public void Dispose()
    {
        this._passivePowerUps?.ForEach(p => p.Dispose());
        this._passivePowerUps = null;
    }

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
        await this.AddLearnedSkillAsync(skillEntry).ConfigureAwait(false);

        if (skill.MasterDefinition?.ReplacedSkill is { } replacedSkill)
        {
            await this._player.InvokeViewPlugInAsync<ISkillListViewPlugIn>(p => p.RemoveSkillAsync(replacedSkill)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<bool> RemoveItemSkillAsync(ushort skillId)
    {
        // The entry is looked up in the item skills, not in the available skills: when the same skill
        // is also learned by the character, the available skills hold the learned entry - removing that
        // one would take a learned skill away just because an item was taken off.
        // This should actually never happen with default configs, but the admin panel allows to configure stuff like that.
        var skillEntry = this._itemSkills.FirstOrDefault(s => s.Skill!.Number.ToUnsigned() == skillId);
        if (skillEntry is null)
        {
            return false;
        }

        this._itemSkills.Remove(skillEntry);

        // We need to take into account that there might be multiple items equipped with the same skill
        if (this._itemSkills.All(s => s.Skill!.Number.ToUnsigned() != skillId)
            && this._availableSkills.TryGetValue(skillId, out var availableSkill)
            && !this._learnedSkills.Contains(availableSkill))
        {
            await this._player.InvokeViewPlugInAsync<ISkillListViewPlugIn>(p => p.RemoveSkillAsync(availableSkill.Skill!)).ConfigureAwait(false);
            this._availableSkills.Remove(skillId);
        }

        return true;
    }

    /// <inheritdoc/>
    public bool ContainsSkill(ushort skillId)
    {
        return this._availableSkills.ContainsKey(skillId);
    }

    /// <summary>
    /// Creates the dictionary of the available skills from the learned skills of the character.
    /// A character may have the same skill in its stored skill list more than once - it's data which
    /// shouldn't exist, but it must never keep the character from entering the game. Such duplicates
    /// are removed from the character, keeping the entry with the highest level.
    /// </summary>
    /// <returns>The dictionary of the available skills.</returns>
    private Dictionary<ushort, SkillEntry> CreateAvailableSkills()
    {
        var availableSkills = new Dictionary<ushort, SkillEntry>();
        foreach (var skillEntry in this._learnedSkills.ToList())
        {
            var skillId = skillEntry.Skill!.Number.ToUnsigned();
            if (availableSkills.TryAdd(skillId, skillEntry))
            {
                continue;
            }

            var previousEntry = availableSkills[skillId];
            var (keptEntry, obsoleteEntry) = skillEntry.Level > previousEntry.Level
                ? (skillEntry, previousEntry)
                : (previousEntry, skillEntry);
            availableSkills[skillId] = keptEntry;
            this._learnedSkills.Remove(obsoleteEntry);

            this._player.Logger.LogWarning(
                "Removed a duplicate learned skill '{Skill}' (number {Number}) of character '{Character}'.",
                skillEntry.Skill.Name,
                skillEntry.Skill.Number,
                this._player.SelectedCharacter?.Name);
        }

        return availableSkills;
    }

    private async ValueTask AddItemSkillAsync(Skill skill)
    {
        // If character is not selected (e.g., during disconnect cleanup), skill list doesn't need to be updated
        if (this._player.SelectedCharacter is null)
        {
            return;
        }

        if (!skill.QualifiedCharacters.Contains(this._player.SelectedCharacter.CharacterClass!))
        {
            return;
        }

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
        // A learned skill replaces the entry of an equipped item which grants the same skill: an item
        // skill is always level 0, while the learned one keeps its own level. The item skill entry stays
        // in the item skill list, so unequipping the item doesn't take the learned skill away.
        this._availableSkills[skill.Skill!.Number.ToUnsigned()] = skill;
        this._learnedSkills.Add(skill);

        if (skill.Skill.SkillType == SkillType.PassiveBoost || this._castedSkillsWithPassiveBoost.Contains(skill.Skill.Number))
        {
            this.CreatePowerUpForPassiveSkill(skill);
        }

        if (skill.Skill.SkillType != SkillType.PassiveBoost)
        {
            await this._player.InvokeViewPlugInAsync<ISkillListViewPlugIn>(p => p.AddSkillAsync(skill.Skill)).ConfigureAwait(false);
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

        var passiveBoost = new PassiveSkillBoostPowerUp(skillEntry);
        this.PassivePowerUps.Add(passiveBoost);
        this.PassivePowerUps.Add(new PowerUpWrapper(passiveBoost, masterDefinition.TargetAttribute, this._player.Attributes!));

        if (skillEntry.Skill.Number == DurabilityReduction1SkillId || skillEntry.Skill.Number == DurabilityReduction1FistMasterSkillId)
        {
            var durabilityReductionFactorBoost = new PassiveSkillBoostPowerUp(skillEntry, true);
            this.PassivePowerUps.Add(durabilityReductionFactorBoost);
            this.PassivePowerUps.Add(new PowerUpWrapper(durabilityReductionFactorBoost, Stats.DurabilityReductionFactor, this._player.Attributes!));
        }
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
            await this.AddItemSkillAsync(item.Definition.Skill).ConfigureAwait(false);
        }
        else
        {
            await this.RemoveItemSkillAsync(item.Definition.Skill.Number.ToUnsigned()).ConfigureAwait(false);
        }
    }

    private sealed class PassiveSkillBoostPowerUp : IElement, IDisposable
    {
        private readonly SkillEntry _skillEntry;

        public PassiveSkillBoostPowerUp(SkillEntry skillEntry, bool isDurabilityReductionFactor = false)
        {
            this._skillEntry = skillEntry;

            if (isDurabilityReductionFactor)
            {
                this.Value = -this._skillEntry.Level / 500f;
                this.AggregateType = AggregateType.AddRaw;
                this._skillEntry.PropertyChanged += this.OnDurabilityReductionSkillEntryOnPropertyChanged;
            }
            else
            {
                this.Value = this._skillEntry.CalculateValue();
                this.AggregateType = this._skillEntry.Skill!.MasterDefinition!.Aggregation;
                this._skillEntry.PropertyChanged += this.OnSkillEntryOnPropertyChanged;
            }
        }

        public event EventHandler? ValueChanged;

        public float Value { get; private set; }

        public AggregateType AggregateType { get; }

        public void Dispose()
        {
            this._skillEntry.PropertyChanged -= this.OnSkillEntryOnPropertyChanged;
            this._skillEntry.PropertyChanged -= this.OnDurabilityReductionSkillEntryOnPropertyChanged;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Value} ({this.AggregateType})";
        }

        private void OnSkillEntryOnPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == nameof(SkillEntry.Level))
            {
                this.Value = this._skillEntry.CalculateValue();
                this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnDurabilityReductionSkillEntryOnPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == nameof(SkillEntry.Level))
            {
                this.Value = -this._skillEntry.Level / 500f;
                this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}