// <copyright file="EffectTimeEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// A snapshot of a magic effect and whether it sends its remaining time to the client, see <see cref="EffectTimeOperations"/>.
/// </summary>
/// <param name="Id">The identifier of the magic effect definition.</param>
/// <param name="Number">The number of the effect.</param>
/// <param name="Name">The name of the effect.</param>
/// <param name="SendsTime">A value indicating whether the effect sends its remaining time to the client.</param>
/// <param name="Skills">
/// The names of the skills which cause the effect. Several effects can have the same number and name, e.g. each ice skill has its own
/// "Iced" effect with its own duration; the skills tell them apart.
/// </param>
public sealed record EffectTimeEntry(Guid Id, short Number, string Name, bool SendsTime, IReadOnlyList<string> Skills)
{
    /// <summary>
    /// Determines whether the number, the name or the name of a skill of the effect contains the text.
    /// </summary>
    /// <param name="text">The text, case-insensitive; an empty text matches every effect.</param>
    /// <returns><c>true</c>, if the effect matches the text.</returns>
    public bool Matches(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        var trimmed = text.Trim();
        return this.Name.Contains(trimmed, StringComparison.OrdinalIgnoreCase)
               || this.Number.ToString(System.Globalization.CultureInfo.InvariantCulture).Contains(trimmed, StringComparison.Ordinal)
               || this.Skills.Any(skill => skill.Contains(trimmed, StringComparison.OrdinalIgnoreCase));
    }
}
