// <copyright file="ConfigurationSearchHit.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>A scalar configuration search result.</summary>
/// <param name="Caption">The caption.</param>
/// <param name="Path">The path.</param>
/// <param name="Url">The url.</param>
public sealed record ConfigurationSearchHit(string Caption, string Path, string Url);
