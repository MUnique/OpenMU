// <copyright file="CharacterRankingRow.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// A row of the query of the character ranking.
/// </summary>
/// <param name="Name">The name of the character.</param>
/// <param name="CharacterClassId">The identifier of the character class; the class itself is configuration, so it can't be joined here.</param>
/// <param name="Level">The level.</param>
/// <param name="MasterLevel">The master level.</param>
/// <param name="Resets">The resets.</param>
internal sealed record CharacterRankingRow(string Name, Guid? CharacterClassId, float Level, float MasterLevel, float Resets);
