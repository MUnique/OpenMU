// <copyright file="SkillListField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A component that displays and allows editing a character's skill list, including the master skill tree.
/// </summary>
public partial class SkillListField : InputBase<ICollection<SkillEntry>>
{
    private Character? _character;
    private IList<MasterSkillRoot>? _masterSkillRoots;
    private IList<Skill>? _availableMasterSkills;

    /// <summary>
    /// Gets or sets the game configuration data source used to retrieve available skills.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal service for opening creation dialogs.
    /// </summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    private IEnumerable<SkillEntry> RegularSkills =>
        (this.Value ?? Enumerable.Empty<SkillEntry>())
        .Where(e => e.Skill?.MasterDefinition == null)
        .OrderBy(e => e.Skill?.GetName());

    private IList<MasterSkillRoot> MasterSkillRoots
    {
        get
        {
            if (this._masterSkillRoots is null)
            {
                this.LoadMasterSkillData();
            }

            return this._masterSkillRoots ?? new List<MasterSkillRoot>();
        }
    }

    private IList<Skill> AvailableMasterSkills
    {
        get
        {
            if (this._availableMasterSkills is null)
            {
                this.LoadMasterSkillData();
            }

            return this._availableMasterSkills ?? new List<Skill>();
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Extract the Character object from the ValueExpression (e.g. () => character.LearnedSkills)
        if (this.ValueExpression?.Body is MemberExpression { Expression: ConstantExpression { Value: { } owner } }
            && owner is Character character
            && character != this._character)
        {
            this._character = character;

            // Reset cached data when the character changes
            this._masterSkillRoots = null;
            this._availableMasterSkills = null;
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out ICollection<SkillEntry> result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotImplementedException();
    }

    private void LoadMasterSkillData()
    {
        var allSkills = this.GameConfigurationSource.GetAll<Skill>();
        var masterSkills = allSkills.Where(s => s.MasterDefinition != null);

        if (this._character?.CharacterClass is { } characterClass)
        {
            masterSkills = masterSkills.Where(s => s.QualifiedCharacters.Contains(characterClass));
        }

        this._availableMasterSkills = masterSkills
            .OrderBy(s => s.MasterDefinition!.Rank)
            .ThenBy(s => s.Number)
            .ToList();

        this._masterSkillRoots = this._availableMasterSkills
            .Select(s => s.MasterDefinition!.Root)
            .Where(r => r is not null)
            .Distinct()
            .Cast<MasterSkillRoot>()
            .ToList();
    }

    private IEnumerable<byte> GetRanksForRoot(MasterSkillRoot root)
    {
        return this.AvailableMasterSkills
            .Where(s => s.MasterDefinition?.Root == root)
            .Select(s => s.MasterDefinition!.Rank)
            .Distinct()
            .OrderBy(r => r);
    }

    private IEnumerable<Skill> GetSkillsForRootAndRank(MasterSkillRoot root, byte rank)
    {
        return this.AvailableMasterSkills
            .Where(s => s.MasterDefinition?.Root == root && s.MasterDefinition?.Rank == rank)
            .OrderBy(s => s.Number);
    }

    private int GetMasterSkillLevel(Skill skill)
    {
        return (this.Value ?? Enumerable.Empty<SkillEntry>())
            .FirstOrDefault(e => e.Skill?.Number == skill.Number)?.Level ?? 0;
    }

    private string GetRequiredSkillNames(Skill skill)
    {
        var required = skill.MasterDefinition?.RequiredMasterSkills;
        if (required is null || !required.Any())
        {
            return string.Empty;
        }

        return string.Join(", ", required.Select(s => s.GetName()));
    }

    private async Task OnMasterSkillLevelChangedAsync(Skill skill, ChangeEventArgs args)
    {
        if (!int.TryParse(args.Value?.ToString(), out var level))
        {
            return;
        }

        level = Math.Max(0, Math.Min(level, skill.MasterDefinition!.MaximumLevel));

        var collection = this.Value;
        if (collection is null)
        {
            return;
        }

        var existingEntry = collection.FirstOrDefault(e => e.Skill?.Number == skill.Number);

        if (level == 0 && existingEntry is not null)
        {
            collection.Remove(existingEntry);
            if (this.PersistenceContext.IsSupporting(typeof(SkillEntry)))
            {
                await this.PersistenceContext.DeleteAsync(existingEntry).ConfigureAwait(false);
            }
        }
        else if (level > 0)
        {
            if (existingEntry is not null)
            {
                existingEntry.Level = level;
            }
            else
            {
                var newEntry = this.PersistenceContext.IsSupporting(typeof(SkillEntry))
                    ? this.PersistenceContext.CreateNew<SkillEntry>()
                    : new SkillEntry();
                newEntry.Skill = skill;
                newEntry.Level = level;
                collection.Add(newEntry);
            }
        }

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private async Task OnResetMasterSkillsClickAsync()
    {
        var collection = this.Value;
        if (collection is null)
        {
            return;
        }

        var masterEntries = collection
            .Where(e => e.Skill?.MasterDefinition is not null)
            .ToList();

        foreach (var entry in masterEntries)
        {
            collection.Remove(entry);
            if (this.PersistenceContext.IsSupporting(typeof(SkillEntry)))
            {
                await this.PersistenceContext.DeleteAsync(entry).ConfigureAwait(false);
            }
        }

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private async Task OnAddRegularSkillClickAsync()
    {
        var newEntry = this.PersistenceContext.IsSupporting(typeof(SkillEntry))
            ? this.PersistenceContext.CreateNew<SkillEntry>()
            : new SkillEntry();

        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<SkillEntry>.Item), newEntry);
        parameters.Add(nameof(ModalCreateNew<SkillEntry>.PersistenceContext), this.PersistenceContext);
        if (this._character is not null)
        {
            parameters.Add(nameof(ModalCreateNew<SkillEntry>.Owner), this._character);
        }

        var options = new ModalOptions { DisableBackgroundCancel = true };
        var modal = this.ModalService.Show<ModalCreateNew<SkillEntry>>("Add Skill", parameters, options);
        var result = await modal.Result.ConfigureAwait(false);

        if (result.Cancelled)
        {
            if (this.PersistenceContext.IsSupporting(typeof(SkillEntry)))
            {
                await this.PersistenceContext.DeleteAsync(newEntry).ConfigureAwait(false);
            }
        }
        else
        {
            this.Value ??= new List<SkillEntry>();
            this.Value.Add(newEntry);
            if (this.PersistenceContext.IsSupporting(typeof(SkillEntry)))
            {
                await this.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
            }

            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task OnRemoveSkillClickAsync(SkillEntry entry)
    {
        this.Value?.Remove(entry);
        if (this.PersistenceContext.IsSupporting(typeof(SkillEntry)))
        {
            await this.PersistenceContext.DeleteAsync(entry).ConfigureAwait(false);
            await this.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }
}
