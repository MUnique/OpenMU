// <copyright file="UnlockRageFighterAtLevel150.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unlocks the Rage Fighter character class when the configured level requirement is met.
/// The level requirement is configured in the CharacterClass.LevelRequirementByCreation property.
/// </summary>
[PlugIn(nameof(UnlockRageFighterAtLevel150), "Unlocks the Rage Fighter character class when the configured level requirement is met.")]
[Guid("2DFFD752-7652-4FA2-93D2-9890CA0F04F2")]
public class UnlockRageFighterAtLevel150 : UnlockCharacterAtLevelBase
{
    private const byte RageFighterNumber = 24;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockRageFighterAtLevel150"/> class.
    /// </summary>
    public UnlockRageFighterAtLevel150()
        : base(RageFighterNumber, "Rage Fighter")
    {
    }
}