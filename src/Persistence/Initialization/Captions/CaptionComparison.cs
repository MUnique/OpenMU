// <copyright file="CaptionComparison.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Captions;

/// <summary>
/// The result of comparing the captions of a configuration with their sources.
/// </summary>
/// <param name="LinkedCaptions">The number of captions which are linked to a source. If it's zero, the captions need to be linked first.</param>
/// <param name="Changes">The differences between the captions and their sources. If it's empty, all available localizations are in place.</param>
/// <param name="UnresolvedSourceKeys">The source keys which can't be resolved.</param>
public sealed record CaptionComparison(int LinkedCaptions, IReadOnlyList<CaptionChange> Changes, IReadOnlyList<string> UnresolvedSourceKeys);
