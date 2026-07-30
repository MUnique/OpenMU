// <copyright file="ExperienceShare.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The experience which a single player gained from a kill.
/// </summary>
/// <param name="Player">The player which gained the experience.</param>
/// <param name="Experience">The gained experience, with all experience rates already applied.</param>
public readonly record struct ExperienceShare(Player Player, int Experience);
