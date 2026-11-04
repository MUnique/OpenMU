// <copyright file="SkillListField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Form.Modal;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Properties;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A component that displays and allows editing a character's skill list, including the master skill tree.
/// </summary>
/// <remarks>
/// Master skills are identified by <see cref="Skill.MasterDefinition"/>. They are shown in a tree,
/// with one column per <see cref="MasterSkillRoot"/> and one row per <see cref="MasterSkillDefinition.Rank"/>.
/// All other skills are shown in a simple list.
/// </remarks>
public partial class SkillListField : InputBase<ICollection<SkillEntry>>
{
    /// <summary>
    /// The minimum level a required master skill (or a skill of the previous rank) needs to have,
    /// so that a master skill can be learned. Matches the rule of the game server.
    /// </summary>
    private const int MinimumLevelOfRequiredSkill = 10;

    private Character? _character;

    private IReadOnlyList<MasterSkillRootColumn>? _masterSkillTree;

    private CharacterClass? _masterSkillTreeClass;

    /// <summary>
    /// A version number which is part of the key of the level inputs.
    /// It's incremented when an entered level was corrected, so that the inputs are re-rendered with the corrected value.
    /// </summary>
    private int _levelInputVersion;

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    [Inject]
    private IModalService ModalService { get; set; } = null!;

    [Inject]
    private IChangeNotificationService NotificationService { get; set; } = null!;

    private IEnumerable<SkillEntry> Entries => this.Value ?? Enumerable.Empty<SkillEntry>();

    private IEnumerable<SkillEntry> RegularSkills =>
        this.Entries
            .Where(e => e.Skill?.MasterDefinition is null)
            .OrderBy(e => e.Skill?.GetName());

    private IEnumerable<SkillEntry> MasterSkillEntries =>
        this.Entries.Where(e => e.Skill?.MasterDefinition is not null);

    private int SpentMasterPoints => this.MasterSkillEntries.Sum(e => e.Level);

    /// <summary>
    /// Gets the master skill tree for the class of the character.
    /// It's rebuilt when the character class changes, because the available master skills depend on it.
    /// </summary>
    private IReadOnlyList<MasterSkillRootColumn> MasterSkillTree
    {
        get
        {
            var characterClass = this._character?.CharacterClass;
            if (this._masterSkillTree is null || this._masterSkillTreeClass != characterClass)
            {
                this._masterSkillTree = this.BuildMasterSkillTree(characterClass);
                this._masterSkillTreeClass = characterClass;
            }

            return this._masterSkillTree;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // The value expression is created by the component builder, e.g. () => character.LearnedSkills.
        this._character = this.ValueExpression?.Body is MemberExpression { Expression: ConstantExpression { Value: Character character } }
            ? character
            : null;
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out ICollection<SkillEntry> result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotImplementedException();
    }

    private static int NormalizeLevel(MasterSkillDefinition definition, int level)
    {
        if (level <= 0)
        {
            return 0;
        }

        // When a master skill is learned in the game, it starts at its minimum level.
        var maximumLevel = Math.Max((int)definition.MaximumLevel, 1);
        var minimumLevel = Math.Clamp((int)definition.MinimumLevel, 1, maximumLevel);
        return Math.Clamp(level, minimumLevel, maximumLevel);
    }

    private IReadOnlyList<MasterSkillRootColumn> BuildMasterSkillTree(CharacterClass? characterClass)
    {
        var masterSkills = this.GameConfigurationSource.GetAll<Skill>()
            .Where(s => s.MasterDefinition?.Root is not null);
        if (characterClass is not null)
        {
            masterSkills = masterSkills.Where(s => s.QualifiedCharacters.Contains(characterClass));
        }

        return masterSkills
            .GroupBy(s => s.MasterDefinition!.Root!)
            .OrderBy(g => g.Min(s => s.Number))
            .Select(rootGroup => new MasterSkillRootColumn(
                rootGroup.Key,
                rootGroup
                    .GroupBy(s => s.MasterDefinition!.Rank)
                    .OrderBy(g => g.Key)
                    .Select(rankGroup => new MasterSkillRankRow(rankGroup.Key, rankGroup.OrderBy(s => s.Number).ToList()))
                    .ToList()))
            .ToList();
    }

    private SkillEntry? GetEntry(Skill skill)
    {
        return this.Entries.FirstOrDefault(e => e.Skill?.Number == skill.Number);
    }

    private int GetLevel(Skill skill)
    {
        return this.GetEntry(skill)?.Level ?? 0;
    }

    private string GetRequiredSkillNames(Skill skill)
    {
        var required = skill.MasterDefinition?.RequiredMasterSkills;
        if (required is null || required.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(", ", required.Select(s => s.GetName()));
    }

    /// <summary>
    /// Determines if the requirements of the game server to learn the master skill are fulfilled by the other learned skills.
    /// </summary>
    /// <param name="skill">The master skill.</param>
    /// <returns><c>true</c>, if the requirements are fulfilled; Otherwise, <c>false</c>.</returns>
    private bool AreRequirementsFulfilled(Skill skill)
    {
        var definition = skill.MasterDefinition!;
        if (definition.Rank > 1
            && !this.MasterSkillEntries.Any(e => e.Skill!.MasterDefinition!.Root?.Id == definition.Root?.Id
                                                 && e.Skill.MasterDefinition.Rank == definition.Rank - 1
                                                 && e.Level >= MinimumLevelOfRequiredSkill))
        {
            return false;
        }

        // Required skills which are no master skills may also be provided by equipped items,
        // so we can only check the master skills here.
        return definition.RequiredMasterSkills
            .Where(required => required.MasterDefinition is not null)
            .All(required => this.GetLevel(required) >= MinimumLevelOfRequiredSkill);
    }

    private string GetEntryCssClass(Skill skill, int level)
    {
        var fulfilled = this.AreRequirementsFulfilled(skill);
        return (level > 0, fulfilled) switch
        {
            (true, true) => "master-skill-active",
            (true, false) => "master-skill-active master-skill-invalid",
            (false, false) => "master-skill-locked",
            _ => string.Empty,
        };
    }

    private string GetEntryTooltip(Skill skill, int level)
    {
        var definition = skill.MasterDefinition!;
        var tooltip = string.Format(Resources.MasterSkillTooltip, skill.GetName(), skill.Number, definition.MinimumLevel, definition.MaximumLevel);
        if (!this.AreRequirementsFulfilled(skill))
        {
            tooltip += Environment.NewLine + string.Format(
                level > 0 ? Resources.MasterSkillRequirementsNotFulfilled : Resources.MasterSkillRequirementsNotYetFulfilled,
                MinimumLevelOfRequiredSkill);
        }

        return tooltip;
    }

    private async Task OnMasterSkillLevelChangedAsync(Skill skill, ChangeEventArgs args)
    {
        var definition = skill.MasterDefinition!;
        var currentLevel = this.GetLevel(skill);
        if (!int.TryParse(args.Value?.ToString(), out var enteredLevel))
        {
            enteredLevel = currentLevel;
        }

        var level = NormalizeLevel(definition, enteredLevel);
        if (level != enteredLevel || args.Value?.ToString() != level.ToString())
        {
            // The input shows a different value than we store, so we need to force a re-render of it.
            this._levelInputVersion++;
        }

        if (level == currentLevel || this.Value is not { } collection)
        {
            return;
        }

        var entry = this.GetEntry(skill);
        if (level > 0)
        {
            if (entry is null)
            {
                entry = this.CreateEntry();
                entry.Skill = skill;
                collection.Add(entry);
            }

            entry.Level = level;
            this.EditContext.NotifyFieldChanged(this.FieldIdentifier);
        }
        else if (entry is not null)
        {
            collection.Remove(entry);
            this.EditContext.NotifyFieldChanged(this.FieldIdentifier);
            await this.DeleteEntryAsync(entry).ConfigureAwait(false);
        }
    }

    private async Task OnResetMasterSkillsClickAsync()
    {
        var masterEntries = this.MasterSkillEntries.ToList();
        if (masterEntries.Count == 0)
        {
            return;
        }

        var spentPoints = masterEntries.Sum(e => e.Level);
        var question = this._character is null
            ? Resources.ResetMasterSkillTreeQuestion
            : string.Format(Resources.ResetMasterSkillTreeAndRefundQuestion, spentPoints);
        if (!await this.ModalService.ShowQuestionAsync(Resources.ResetMasterSkillTree, question).ConfigureAwait(false))
        {
            return;
        }

        await this.InvokeAsync(() =>
        {
            foreach (var entry in masterEntries)
            {
                this.Value?.Remove(entry);
            }

            if (this._character is { } character)
            {
                // Every level of a master skill costs one master level up point, so we give them back.
                character.MasterLevelUpPoints += spentPoints;
                this.NotificationService.NotifyChange(character, nameof(Character.MasterLevelUpPoints));
            }

            this.EditContext.NotifyFieldChanged(this.FieldIdentifier);
            this.StateHasChanged();
        }).ConfigureAwait(false);

        foreach (var entry in masterEntries)
        {
            await this.DeleteEntryAsync(entry).ConfigureAwait(false);
        }
    }

    private async Task OnAddRegularSkillClickAsync()
    {
        var newEntry = this.CreateEntry();

        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<SkillEntry>.Item), newEntry);
        parameters.Add(nameof(ModalCreateNew<SkillEntry>.PersistenceContext), this.PersistenceContext);
        parameters.Add(nameof(ModalCreateNew<SkillEntry>.Owner), this._character);

        var options = new ModalOptions { DisableBackgroundCancel = true };
        var modal = this.ModalService.Show<ModalCreateNew<SkillEntry>>(Resources.AddSkill, parameters, options);
        var result = await modal.Result.ConfigureAwait(false);

        // A skill which is already learned doesn't need to be added twice.
        if (result.Cancelled
            || newEntry.Skill is not { } skill
            || this.Entries.Any(e => e != newEntry && e.Skill?.Number == skill.Number))
        {
            await this.DeleteEntryAsync(newEntry).ConfigureAwait(false);
            return;
        }

        await this.InvokeAsync(() =>
        {
            this.Value ??= new List<SkillEntry>();
            this.Value.Add(newEntry);
            this.EditContext.NotifyFieldChanged(this.FieldIdentifier);
            this.StateHasChanged();
        }).ConfigureAwait(false);
    }

    private async Task OnRemoveSkillClickAsync(SkillEntry entry)
    {
        this.Value?.Remove(entry);
        this.EditContext.NotifyFieldChanged(this.FieldIdentifier);
        await this.DeleteEntryAsync(entry).ConfigureAwait(false);
    }

    private SkillEntry CreateEntry()
    {
        return this.PersistenceContext.IsSupporting(typeof(SkillEntry))
            ? this.PersistenceContext.CreateNew<SkillEntry>()
            : new SkillEntry();
    }

    /// <summary>
    /// Deletes the entry from the persistence context.
    /// Skill entries are members of the character aggregate, so they don't exist without it.
    /// </summary>
    /// <param name="entry">The entry which was removed from the collection or never added to it.</param>
    private async ValueTask DeleteEntryAsync(SkillEntry entry)
    {
        if (this.PersistenceContext.IsSupporting(typeof(SkillEntry)))
        {
            await this.PersistenceContext.DeleteAsync(entry).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// A column of the master skill tree.
    /// </summary>
    /// <param name="Root">The root of the master skills.</param>
    /// <param name="Ranks">The ranks of the master skills of the root.</param>
    private sealed record MasterSkillRootColumn(MasterSkillRoot Root, IReadOnlyList<MasterSkillRankRow> Ranks);

    /// <summary>
    /// A row of a <see cref="MasterSkillRootColumn"/>.
    /// </summary>
    /// <param name="Rank">The rank of the skills.</param>
    /// <param name="Skills">The skills of this rank.</param>
    private sealed record MasterSkillRankRow(byte Rank, IReadOnlyList<Skill> Skills);
}
