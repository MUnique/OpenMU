// <copyright file="ICharacterCreatedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a character has been created.
/// </summary>
/// <remarks>
/// It's called when a player created the character in the game, when the admin panel created it,
/// and when a character which was created on the database enters the game for the first time
/// (see <see cref="Player"/>). As there is not always a player in the game, it gets everything it
/// needs as parameters.
/// </remarks>
[Guid("B5588572-A324-4A94-9644-4DC3C8FEA4A4")]
[PlugInPoint("Character created", "Is called when a character got created.")]
public interface ICharacterCreatedPlugIn
{
    /// <summary>
    /// Is called when a new character has been created.
    /// </summary>
    /// <param name="account">The account of the created character.</param>
    /// <param name="createdCharacter">The created character.</param>
    /// <param name="persistenceContext">The persistence context of the account, which creates new objects for the character, e.g. items.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="logger">The logger.</param>
    void CharacterCreated(Account account, Character createdCharacter, IContext persistenceContext, GameConfiguration gameConfiguration, ILogger logger);
}