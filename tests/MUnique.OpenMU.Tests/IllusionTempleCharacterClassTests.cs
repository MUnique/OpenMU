// <copyright file="IllusionTempleCharacterClassTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView.MiniGames;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Tests the conversion of the internal character class number into the number which the game client
/// expects in the illusion temple result packet.
/// </summary>
/// <remarks>
/// The client reads the class line from the lower nibble and the evolution step from the upper one,
/// while the internal numbering packs both the other way around. The tricky part are the three lines
/// which have no second class - their master class sits at internal step 1, not 3.
/// </remarks>
[TestFixture]
public class IllusionTempleCharacterClassTests
{
    private const byte BaseClass = 0x00;
    private const byte SecondClass = 0x20;
    private const byte MasterClass = 0x30;

    private const byte WizardLine = 0;
    private const byte KnightLine = 1;
    private const byte ElfLine = 2;
    private const byte GladiatorLine = 3;
    private const byte LordLine = 4;
    private const byte SummonerLine = 5;
    private const byte FighterLine = 6;

    /// <summary>
    /// Every character class is converted to the line and evolution step the client expects.
    /// </summary>
    /// <param name="characterClassNumber">The internal character class number.</param>
    /// <param name="expected">The expected value for the client.</param>
    [TestCase((byte)CharacterClassNumber.DarkWizard, BaseClass | WizardLine)]
    [TestCase((byte)CharacterClassNumber.SoulMaster, SecondClass | WizardLine)]
    [TestCase((byte)CharacterClassNumber.GrandMaster, MasterClass | WizardLine)]
    [TestCase((byte)CharacterClassNumber.DarkKnight, BaseClass | KnightLine)]
    [TestCase((byte)CharacterClassNumber.BladeKnight, SecondClass | KnightLine)]
    [TestCase((byte)CharacterClassNumber.BladeMaster, MasterClass | KnightLine)]
    [TestCase((byte)CharacterClassNumber.FairyElf, BaseClass | ElfLine)]
    [TestCase((byte)CharacterClassNumber.MuseElf, SecondClass | ElfLine)]
    [TestCase((byte)CharacterClassNumber.HighElf, MasterClass | ElfLine)]
    [TestCase((byte)CharacterClassNumber.MagicGladiator, BaseClass | GladiatorLine)]
    [TestCase((byte)CharacterClassNumber.DuelMaster, MasterClass | GladiatorLine)]
    [TestCase((byte)CharacterClassNumber.DarkLord, BaseClass | LordLine)]
    [TestCase((byte)CharacterClassNumber.LordEmperor, MasterClass | LordLine)]
    [TestCase((byte)CharacterClassNumber.Summoner, BaseClass | SummonerLine)]
    [TestCase((byte)CharacterClassNumber.BloodySummoner, SecondClass | SummonerLine)]
    [TestCase((byte)CharacterClassNumber.DimensionMaster, MasterClass | SummonerLine)]
    [TestCase((byte)CharacterClassNumber.RageFighter, BaseClass | FighterLine)]
    [TestCase((byte)CharacterClassNumber.FistMaster, MasterClass | FighterLine)]
    public void ConvertsEveryCharacterClass(byte characterClassNumber, int expected)
    {
        Assert.That(characterClassNumber.ToIllusionTempleCharacterClass(), Is.EqualTo((byte)expected));
    }

    /// <summary>
    /// The master classes of the three lines without a second class (Magic Gladiator, Dark Lord and
    /// Rage Fighter) are reported as master classes, and not with the internal step of 1, which the
    /// client doesn't know.
    /// </summary>
    /// <param name="characterClassNumber">The internal character class number of a master class.</param>
    [TestCase((byte)CharacterClassNumber.DuelMaster)]
    [TestCase((byte)CharacterClassNumber.LordEmperor)]
    [TestCase((byte)CharacterClassNumber.FistMaster)]
    public void ReportsMasterClassesOfTwoTierLinesAsMasterClass(byte characterClassNumber)
    {
        var converted = characterClassNumber.ToIllusionTempleCharacterClass();

        Assert.That(converted & 0xF0, Is.EqualTo(MasterClass));
    }
}
