// <copyright file="CharacterSelectionResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.AccountLinking;

/// <summary>
/// The result of selecting the character, as which a linked user appears.
/// </summary>
public enum CharacterSelectionResult
{
    /// <summary>
    /// The character was selected.
    /// </summary>
    Selected,

    /// <summary>
    /// The user isn't linked to an account.
    /// </summary>
    NotLinked,

    /// <summary>
    /// The account has no character with the name.
    /// </summary>
    CharacterNotFound,
}
