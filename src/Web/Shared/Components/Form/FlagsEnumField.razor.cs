// <copyright file="FlagsEnumField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Lookup field that allows to select multiple objects which will be stored in a bound <see cref="IList{TObject}"/>.
/// </summary>
/// <typeparam name="TValue">The type of the enum.</typeparam>
public partial class FlagsEnumField<TValue> : NotifyableInputBase<TValue>
    where TValue : struct, Enum
{
    private static readonly TValue[] PossibleFlags = Enum.GetValues(typeof(TValue)).OfType<TValue>().Where(v => !default(TValue).HasFlag(v)).OrderBy(v => v.ToString()).ToArray();

    private static readonly Dictionary<TValue, string> FlagNames = PossibleFlags.ToDictionary(f => f, f => f.ToString());

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
    private Task OnValueChangedAsync(IList<TValue> flags)
    {
        if (flags.Count == 0)
        {
            this.CurrentValue = default;
        }
        else
        {
            var result = flags.Select(f => Convert.ToInt32(f)).Aggregate((a, b) => a | b);
            this.CurrentValue = (TValue)Enum.ToObject(typeof(TValue), result);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Searches for the available flags, which are not assigned yet to the value.
    /// </summary>
    /// <param name="text">The search text.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The available flags.</returns>
    private Task<IEnumerable<TValue>> SearchAsync(string text, System.Threading.CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<IEnumerable<TValue>>(token);
        }

        var results = this.UnassignedFlags.Where(f => FlagNames[f].Contains(text, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(results);
    }
}