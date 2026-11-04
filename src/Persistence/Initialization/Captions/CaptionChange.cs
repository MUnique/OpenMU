// <copyright file="CaptionChange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Captions;

/// <summary>
/// The kind of a <see cref="CaptionChange"/>.
/// </summary>
public enum CaptionChangeKind
{
    /// <summary>
    /// The source provides a translation which is missing in the configuration, or the configuration
    /// only contains a copy of the neutral text.
    /// </summary>
    Missing,

    /// <summary>
    /// The source text was changed, and the configured text is still unchanged since it was taken from the source.
    /// </summary>
    Updated,

    /// <summary>
    /// The source doesn't provide this translation anymore, and the configured text is still unchanged since it was taken from the source.
    /// </summary>
    Removed,

    /// <summary>
    /// The configured text differs from the source and was customized, or its origin is unknown.
    /// </summary>
    Customized,
}

/// <summary>
/// A difference between a configured caption and its source.
/// </summary>
/// <param name="Id">The identifier of the change, which is stable across contexts.</param>
/// <param name="OwnerId">The identifier of the object which owns the caption.</param>
/// <param name="OwnerType">The name of the type of the object which owns the caption.</param>
/// <param name="PropertyName">The name of the caption property.</param>
/// <param name="SourceKey">The source key, see <see cref="Interfaces.LocalizedString.SourceKey"/>.</param>
/// <param name="NeutralText">The current neutral text of the caption, to identify it.</param>
/// <param name="CultureName">The name of the culture, or <see langword="null"/> for the neutral text.</param>
/// <param name="CurrentText">The currently configured text.</param>
/// <param name="SourceText">The text of the source.</param>
/// <param name="Kind">The kind of the change.</param>
public sealed record CaptionChange(
    string Id,
    Guid OwnerId,
    string OwnerType,
    string PropertyName,
    string SourceKey,
    string NeutralText,
    string? CultureName,
    string? CurrentText,
    string? SourceText,
    CaptionChangeKind Kind)
{
    /// <summary>
    /// Gets a value indicating whether applying this change is recommended,
    /// because it doesn't overwrite customized texts.
    /// </summary>
    public bool IsRecommended => this.Kind is not CaptionChangeKind.Customized;
}
