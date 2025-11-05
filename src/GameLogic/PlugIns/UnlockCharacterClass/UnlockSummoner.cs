// <copyright file="UnlockSummoner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unlocks the Summoner character class when the configured level requirement is met.
/// The level requirement is configured in the CharacterClass.LevelRequirementByCreation property.
/// </summary>
[PlugIn(nameof(UnlockSummoner), "Unlocks the Summoner character class when the configured level requirement is met.")]
[Guid("2DFFD751-7651-4FA1-93D1-9890CA0F04F1")]
public class UnlockSummoner : UnlockCharacterAtLevelBase
{
    private const byte SummonerNumber = 20;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockSummoner"/> class.
    /// </summary>
    public UnlockSummoner()
        : base(SummonerNumber, "Summoner")
    {
    }
}