// <copyright file="DuelVariant.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The variant of the duel system, which defines where a duel takes place.
/// </summary>
/// <remarks>
/// Both variants existed in the original game: before Season 4, a duel was fought where the
/// duelists were standing. With Season 4, the duel arena was introduced, where the duelists
/// fight undisturbed and other players can watch them.
/// </remarks>
public enum DuelVariant
{
    /// <summary>
    /// The duelists are teleported into a free area of the duel arena, where other players
    /// can watch the duel as invisible spectators.
    /// </summary>
    DuelArena = 0,

    /// <summary>
    /// The duel takes place on the map where the duelists are standing, with all other players
    /// and monsters around. Spectators are not supported by this variant, because everyone
    /// nearby sees the duel anyway.
    /// </summary>
    CurrentMap = 1,
}
