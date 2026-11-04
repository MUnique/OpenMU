// <copyright file="ConfigurationSearchEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A precomputed configuration-search entry.
/// </summary>
/// <param name="Caption">The display caption.</param>
/// <param name="Path">The full path in the configuration graph.</param>
/// <param name="Url">The target edit URL.</param>
/// <param name="NormalizedHaystack">The normalized searchable text.</param>
/// <param name="NormalizedCaption">The normalized caption.</param>
public sealed record ConfigurationSearchEntry(
    string Caption,
    string Path,
    string Url,
    string NormalizedHaystack,
    string NormalizedCaption);
