// <copyright file="UnlockMagicGladiator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unlocks the Magic Gladiator character class when the configured level requirement is met.
/// The level requirement is configured in the CharacterClass.LevelRequirementByCreation property.
/// </summary>
[PlugIn(nameof(UnlockMagicGladiator), "Unlocks the Magic Gladiator character class when the configured level requirement is met.")]
[Guid("8C765FF3-B574-41C6-9151-ABC10D3FD959")]
public class UnlockMagicGladiator : UnlockCharacterAtLevelBase
{
    private const byte MagicGladiatorNumber = 12;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockMagicGladiator"/> class.
    /// </summary>
    public UnlockMagicGladiator()
        : base(MagicGladiatorNumber, "Magic Gladiator")
    {
    }
}