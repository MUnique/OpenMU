// <copyright file="ICharacterMasterLevelUpPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a <see cref="Player"/> gained a master level.
/// </summary>
/// <remarks>
/// It's the counterpart of <see cref="ICharacterLevelUpPlugIn"/> for the master level.
/// </remarks>
[Guid("4F63B5A9-2E4E-4F0F-8A6B-19B7A9E1D4C3")]
[PlugInPoint("Player gained master level", "Plugins which will be executed when a player gained a master level.")]
public interface ICharacterMasterLevelUpPlugIn
{
    /// <summary>
    /// This method is called when a <see cref="Player"/> gained a master level.
    /// </summary>
    /// <param name="player">The player.</param>
    ValueTask CharacterMasterLeveledUpAsync(Player player);
}
