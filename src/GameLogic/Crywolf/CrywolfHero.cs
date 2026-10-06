// <copyright file="CrywolfHero.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// A hero of the crywolf event, which is one of the players with the highest scores.
/// </summary>
/// <param name="Name">The name of the character.</param>
/// <param name="Score">The score.</param>
/// <param name="CharacterClassNumber">The number of the character class.</param>
public record CrywolfHero(string Name, int Score, byte CharacterClassNumber);
