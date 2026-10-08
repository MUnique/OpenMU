// <copyright file="CharacterRankingRow.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// A row of the query of the character ranking.
/// </summary>
/// <param name="Name">The name of the character.</param>
/// <param name="ClassName">The name of the character class.</param>
/// <param name="Level">The level.</param>
/// <param name="MasterLevel">The master level.</param>
/// <param name="Resets">The resets.</param>
internal sealed record CharacterRankingRow(string Name, LocalizedString ClassName, float Level, float MasterLevel, float Resets);
