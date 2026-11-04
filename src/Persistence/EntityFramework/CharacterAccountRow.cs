// <copyright file="CharacterAccountRow.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// A character name with the identifier of its account.
/// </summary>
/// <param name="Name">The name of the character.</param>
/// <param name="AccountId">The identifier of the account.</param>
internal sealed record CharacterAccountRow(string Name, Guid AccountId);
