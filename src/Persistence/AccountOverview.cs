// <copyright file="AccountOverview.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>A narrow account and character projection for owner-scoped portal reads.</summary>
/// <param name="LoginName">The account's login name.</param>
/// <param name="State">The current account state.</param>
/// <param name="Characters">The account's character summaries.</param>
public sealed record AccountOverview(string LoginName, AccountState State, IReadOnlyList<AccountCharacterOverview> Characters);

/// <summary>A character projection containing only fields needed by the player portal.</summary>
/// <param name="Name">The character name.</param>
/// <param name="CharacterSlot">The character slot.</param>
/// <param name="CharacterClassId">The configured character class identifier.</param>
/// <param name="Level">The character level.</param>
/// <param name="MasterLevel">The character master level.</param>
public sealed record AccountCharacterOverview(string Name, byte CharacterSlot, Guid? CharacterClassId, float Level, float MasterLevel);
