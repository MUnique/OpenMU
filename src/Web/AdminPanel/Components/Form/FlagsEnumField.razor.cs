// <copyright file="FlagsEnumField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Lookup field which allows to select multiple objects which will be stored in a bound <see cref="IList{TObject}"/>.
/// </summary>
public partial class FlagsEnumField<TValue> : NotifyableInputBase<TValue>
    where TValue : struct, Enum
{
    private static readonly TValue[] PossibleFlags = Enum.GetValues(typeof(TValue)).OfType<TValue>().Where(v => !default(TValue).HasFlag(v)).OrderBy(v => v.ToString()).ToArray();

    /// <summary>
    /// Gets or sets the label which should be displayed. If it's not explicitly provided, the component shows the
    /// Name defined in the <see cref="DisplayAttribute"/>. If there is no Name in a <see cref="DisplayAttribute"/>, it shows the property name instead.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    private string Placeholder => this.UnassignedFlags.Any() ? "Add ..." : "No more available";

    private IEnumerable<TValue> UnassignedFlags => PossibleFlags.Where(f => !this.Value.HasFlag(f));

    private IList<TValue> ValueAsSingleFlags
    {
        get
        {
            return PossibleFlags.Where(possible => this.Value.HasFlag(possible)).ToList();
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out TValue result, out string validationErrorMessage)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when the selected flags changed.
    /// Combines the flags into one value of this component.
    /// </summary>
    /// <param name="flags">The selected flags.</param>
    private async Task OnValueChangedAsync(IList<TValue> flags)
    {
        if (flags.Count == 0)
        {
            this.CurrentValue = default;
        }
        else
        {
            var result = flags.Cast<int>().Aggregate((a, b) => a | b);
            this.CurrentValue = (TValue)(object)result;
        }
    }

    /// <summary>
    /// Searches for the available flags, which are not assigned yet to the value.
    /// </summary>
    /// <param name="text">The search text.</param>
    /// <returns>The available flags.</returns>
    private async Task<IEnumerable<TValue>> SearchAsync(string text)
    {
        return this.UnassignedFlags.Where(f => f.ToString().Contains(text, StringComparison.InvariantCultureIgnoreCase));
    }
}