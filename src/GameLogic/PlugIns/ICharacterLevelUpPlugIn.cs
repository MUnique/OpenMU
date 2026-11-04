// <copyright file="ICharacterLevelUpPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a <see cref="Player"/> leveled up.
/// </summary>
[Guid("049D9A94-504B-4BED-9DC9-13793F8AC7E4")]
[PlugInPoint("Player leveled up", "Plugins which will be executed when a player leveled up.")]
public interface ICharacterLevelUpPlugIn
{
    /// <summary>
    /// This method is called when a <see cref="Player"/> leveled up.
    /// </summary>
    /// <param name="player">The player.</param>
    void CharacterLeveledUp(Player player);
}