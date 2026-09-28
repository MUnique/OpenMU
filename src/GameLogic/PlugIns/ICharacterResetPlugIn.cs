// <copyright file="ICharacterResetPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a <see cref="Player"/> did a reset of the character.
/// </summary>
[Guid("5E0E7B9C-3A0F-4B46-9C3B-6F5B8A2D9E41")]
[PlugInPoint("Character reset", "Plugins which will be executed when a character has been reset.")]
public interface ICharacterResetPlugIn
{
    /// <summary>
    /// Is called when the character of a <see cref="Player"/> has been reset.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="resetCount">The reset count after the reset.</param>
    ValueTask CharacterResetAsync(Player player, int resetCount);
}
