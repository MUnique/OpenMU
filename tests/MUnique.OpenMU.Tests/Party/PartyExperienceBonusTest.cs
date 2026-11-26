// <copyright file="PartyExperienceBonusTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using CharacterClass = MUnique.OpenMU.Persistence.BasicModel.CharacterClass;

/// <summary>
/// Tests the configurable experience bonus of parties.
/// </summary>
[TestFixture]
public class PartyExperienceBonusTest
{
    // The values which the data initializations of all versions configure.
    private static readonly (AttributeDefinition Attribute, float Value)[] DefaultBonuses =
    [
        (Stats.PartyExperienceMultiplier, 1.05f),
        (Stats.SetPartyExperienceBonus, 0f),
    ];

    private IGameContext _gameContext = null!;
    private CharacterClass _darkWizard = null!;
    private CharacterClass _grandMaster = null!;
    private CharacterClass _darkKnight = null!;
    private CharacterClass _fairyElf = null!;
    private CharacterClass _magicGladiator = null!;
    private CharacterClass _darkLord = null!;

    /// <summary>
    /// Sets up a game context with two generations of the dark wizard and dark knight lines, and further first generation classes.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
        this._grandMaster = new CharacterClass { Number = 2 };
        this._darkWizard = new CharacterClass { Number = 0, NextGenerationClass = this._grandMaster };
        var blademaster = new CharacterClass { Number = 18 };
        this._darkKnight = new CharacterClass { Number = 16, NextGenerationClass = blademaster };
        this._fairyElf = new CharacterClass { Number = 32 };
        this._magicGladiator = new CharacterClass { Number = 48 };
        this._darkLord = new CharacterClass { Number = 64 };

        var classes = this._gameContext.Configuration.CharacterClasses;
        classes.Add(this._darkWizard);
        classes.Add(this._grandMaster);
        classes.Add(this._darkKnight);
        classes.Add(blademaster);
        classes.Add(this._fairyElf);
        classes.Add(this._magicGladiator);
        classes.Add(this._darkLord);
    }

    /// <summary>
    /// Tests that a single player doesn't get a party bonus.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask SinglePlayerGetsNoBonusAsync()
    {
        var members = await this.CreateMembersAsync(this._darkWizard).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.0));
    }

    /// <summary>
    /// Tests that the default configuration, which is the one of the data initializations, keeps the party experience
    /// of the original implementation: <c>1.05^(n-1)</c>. The calculated multiplier needs to be exactly the same
    /// as the one of <c>Math.Pow(1.05, memberCount - 1)</c>.
    /// </summary>
    /// <param name="memberCount">The member count.</param>
    /// <param name="expectedMultiplier">The expected multiplier, as literal.</param>
    /// <returns>The async task.</returns>
    [TestCase(2, 1.05)]
    [TestCase(3, 1.1025)]
    [TestCase(4, 1.157625)]
    [TestCase(5, 1.21550625)]
    public async ValueTask DefaultsKeepCompoundingPartyExperienceAsync(int memberCount, double expectedMultiplier)
    {
        var members = await this.CreateMembersAsync(Enumerable.Repeat(this._darkWizard, memberCount).ToArray()).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.False);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(Math.Pow(1.05, memberCount - 1)));
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(expectedMultiplier).Within(0.000000001));
    }

    /// <summary>
    /// Tests that parties which are bigger than five members still get the bonus for every member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask DefaultsKeepCompoundingForBigPartiesAsync()
    {
        var members = await this.CreateMembersAsync(Enumerable.Repeat(this._darkWizard, 6).ToArray()).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(Math.Pow(1.05, 5)));
    }

    /// <summary>
    /// Tests that the default configuration doesn't give set parties an additional bonus, because the set party bonus is 0.
    /// </summary>
    /// <param name="memberCount">The member count.</param>
    /// <returns>The async task.</returns>
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    public async ValueTask DefaultsGiveSetPartiesNoExtraBonusAsync(int memberCount)
    {
        var members = await this.CreateMembersAsync(this.GetDifferentClasses(memberCount)).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(Math.Pow(1.05, memberCount - 1)));
    }

    /// <summary>
    /// Tests that the multiplier is taken from the configured attribute value, and not hard-coded.
    /// </summary>
    /// <param name="partyMultiplier">The configured party experience multiplier.</param>
    /// <param name="memberCount">The member count.</param>
    /// <param name="expectedMultiplier">The expected multiplier.</param>
    /// <returns>The async task.</returns>
    [TestCase(1.1f, 2, 1.1)]
    [TestCase(1.1f, 3, 1.21)]
    [TestCase(1.02f, 5, 1.08243216)]
    [TestCase(1.0f, 5, 1.0)]
    public async ValueTask ConfiguredPartyMultiplierIsUsedAsync(float partyMultiplier, int memberCount, double expectedMultiplier)
    {
        var members = await this.CreateMembersAsync(false, Enumerable.Repeat(this._darkWizard, memberCount).ToArray()).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(partyMultiplier, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);

        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(expectedMultiplier).Within(0.000001));
    }

    /// <summary>
    /// Tests that a party multiplier of zero or less disables the party bonus.
    /// </summary>
    /// <param name="partyMultiplier">The configured party experience multiplier.</param>
    /// <returns>The async task.</returns>
    [TestCase(0f)]
    [TestCase(-1.05f)]
    public async ValueTask NonPositivePartyMultiplierDisablesBonusAsync(float partyMultiplier)
    {
        var members = await this.CreateMembersAsync(false, this._darkWizard, this._darkWizard, this._darkWizard).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(partyMultiplier, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);

        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.0));
    }

    /// <summary>
    /// Tests that a party gets no bonus if the attributes are not configured at all.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask NotConfiguredBonusIsNoBonusAsync()
    {
        var members = await this.CreateMembersAsync(false, this._darkWizard, this._darkKnight, this._fairyElf).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.0));
    }

    /// <summary>
    /// Tests that the set party bonus is added to the multiplier of a set party, if it's configured.
    /// </summary>
    /// <param name="memberCount">The member count.</param>
    /// <returns>The async task.</returns>
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    public async ValueTask SetPartyBonusIsAddedAsync(int memberCount)
    {
        var members = await this.CreateMembersAsync(this.GetDifferentClasses(memberCount)).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(0.02f, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(Math.Pow(1.05, memberCount - 1) + 0.02).Within(0.000001));
    }

    /// <summary>
    /// Tests that the set party bonus is also added for parties which are bigger than five members.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask SetPartyBonusIsAddedForBigPartiesAsync()
    {
        var members = await this.CreateMembersAsync(
            this._darkWizard,
            this._darkKnight,
            this._fairyElf,
            this._magicGladiator,
            this._darkLord,
            new CharacterClass { Number = 80 }).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(0.02f, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(Math.Pow(1.05, 5) + 0.02).Within(0.000001));
    }

    /// <summary>
    /// Tests that a non-positive set party bonus is ignored and doesn't reduce the experience.
    /// </summary>
    /// <param name="setPartyBonus">The configured set party bonus.</param>
    /// <returns>The async task.</returns>
    [TestCase(0f)]
    [TestCase(-0.02f)]
    public async ValueTask NonPositiveSetPartyBonusIsIgnoredAsync(float setPartyBonus)
    {
        var members = await this.CreateMembersAsync(this._darkWizard, this._darkKnight, this._fairyElf).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(setPartyBonus, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(Math.Pow(1.05, 2)));
    }

    /// <summary>
    /// Tests that the set party bonus is not added to parties with less than three members, even if they have different classes.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask TwoDifferentClassesAreNoSetPartyAsync()
    {
        var members = await this.CreateMembersAsync(this._darkWizard, this._darkKnight).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(0.02f, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.False);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.05));
    }

    /// <summary>
    /// Tests that the members of a set party can be of a different generation of their class line.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask SetPartyWithEvolvedClassesAsync()
    {
        var members = await this.CreateMembersAsync(this._grandMaster, this._darkKnight, this._fairyElf).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
    }

    /// <summary>
    /// Tests that two members of the same class line, even of different generations, are no set party,
    /// so they don't get the set party bonus.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask SameClassLineIsNoSetPartyAsync()
    {
        var members = await this.CreateMembersAsync(this._darkWizard, this._grandMaster, this._fairyElf).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(0.02f, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.False);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(Math.Pow(1.05, 2)));
    }

    private CharacterClass[] GetDifferentClasses(int count)
    {
        var classes = new[] { this._darkWizard, this._darkKnight, this._fairyElf, this._magicGladiator, this._darkLord };
        return classes.Take(count).ToArray();
    }

    private ValueTask<List<Player>> CreateMembersAsync(params CharacterClass[] classes)
    {
        return this.CreateMembersAsync(true, classes);
    }

    private async ValueTask<List<Player>> CreateMembersAsync(bool addDefaultBonuses, params CharacterClass[] classes)
    {
        var result = new List<Player>(classes.Length);
        foreach (var characterClass in classes)
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
            player.SelectedCharacter!.CharacterClass = characterClass;
            if (addDefaultBonuses)
            {
                foreach (var (attribute, value) in DefaultBonuses)
                {
                    player.Attributes!.AddElement(new SimpleElement(value, AggregateType.AddRaw), attribute);
                }
            }

            result.Add(player);
        }

        return result;
    }
}
