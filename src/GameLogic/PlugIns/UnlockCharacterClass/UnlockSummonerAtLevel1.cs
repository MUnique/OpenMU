// <copyright file="UnlockSummonerAtLevel1.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unlocks the Summoner character class as soon as the first character of an account reaches level 1.
/// TODO: Add configuration for the level and change name; remove the "AtLevel1" suffix.
/// </summary>
[PlugIn(nameof(UnlockSummonerAtLevel1), "Unlocks the Summoner character class as soon as the first character of an account reaches level 1.")]
[Guid("2DFFD751-7651-4FA1-93D1-9890CA0F04F1")]
public class UnlockSummonerAtLevel1 : UnlockCharacterAtLevelBase
{
    private const byte SummonerNumber = 20;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockSummonerAtLevel1"/> class.
    /// </summary>
    public UnlockSummonerAtLevel1()
        : base(SummonerNumber, 1, "Summoner")
    {
    }
}
