// <copyright file="PartyExperienceBonusTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using CharacterClass = MUnique.OpenMU.Persistence.BasicModel.CharacterClass;

/// <summary>
/// Tests the configurable experience bonus of general parties and set parties.
/// </summary>
[TestFixture]
public class PartyExperienceBonusTest
{
    private IGameContext _gameContext = null!;
    private CharacterClass _darkWizard = null!;
    private CharacterClass _grandMaster = null!;
    private CharacterClass _darkKnight = null!;
    private CharacterClass _blademaster = null!;
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
        this._blademaster = new CharacterClass { Number = 18 };
        this._darkKnight = new CharacterClass { Number = 16, NextGenerationClass = this._blademaster };
        this._fairyElf = new CharacterClass { Number = 32 };
        this._magicGladiator = new CharacterClass { Number = 48 };
        this._darkLord = new CharacterClass { Number = 64 };

        var classes = this._gameContext.Configuration.CharacterClasses;
        classes.Add(this._darkWizard);
        classes.Add(this._grandMaster);
        classes.Add(this._darkKnight);
        classes.Add(this._blademaster);
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
    /// Tests the default bonus of general parties, which are parties with members of the same class.
    /// </summary>
    /// <param name="memberCount">The member count.</param>
    /// <param name="expectedBonus">The expected bonus.</param>
    /// <returns>The async task.</returns>
    [TestCase(2, 0.02)]
    [TestCase(3, 0.03)]
    [TestCase(4, 0.04)]
    [TestCase(5, 0.05)]
    public async ValueTask GeneralPartyBonusAsync(int memberCount, double expectedBonus)
    {
        var members = await this.CreateMembersAsync(Enumerable.Repeat(this._darkWizard, memberCount).ToArray()).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.False);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.0 + expectedBonus).Within(0.0001));
    }

    /// <summary>
    /// Tests that two members with different classes are not a set party, because it needs at least three members.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask TwoDifferentClassesAreNoSetPartyAsync()
    {
        var members = await this.CreateMembersAsync(this._darkWizard, this._darkKnight).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.False);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.02).Within(0.0001));
    }

    /// <summary>
    /// Tests the default bonus of set parties, which are parties with members of different class lines.
    /// The set party bonus is disabled by default, so they get the general bonus.
    /// </summary>
    /// <param name="memberCount">The member count.</param>
    /// <param name="expectedBonus">The expected bonus.</param>
    /// <returns>The async task.</returns>
    [TestCase(3, 0.03)]
    [TestCase(4, 0.04)]
    [TestCase(5, 0.05)]
    public async ValueTask SetPartyBonusAsync(int memberCount, double expectedBonus)
    {
        var classes = new[] { this._darkWizard, this._darkKnight, this._fairyElf, this._magicGladiator, this._darkLord };
        var members = await this.CreateMembersAsync(classes.Take(memberCount).ToArray()).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.0 + expectedBonus).Within(0.0001));
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
    /// Tests that two members of the same class line, even of different generations, are no set party.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask SameClassLineIsNoSetPartyAsync()
    {
        var members = await this.CreateMembersAsync(this._darkWizard, this._grandMaster, this._fairyElf).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.False);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.03).Within(0.0001));
    }

    /// <summary>
    /// Tests that parties which are bigger than five members still get the bonus for every member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask PartyBiggerThanFiveMembersGetsBonusForEveryMemberAsync()
    {
        var general = await this.CreateMembersAsync(Enumerable.Repeat(this._darkWizard, 6).ToArray()).ConfigureAwait(false);
        var set = await this.CreateMembersAsync(
            this._darkWizard,
            this._darkKnight,
            this._fairyElf,
            this._magicGladiator,
            this._darkLord,
            new CharacterClass { Number = 80 }).ConfigureAwait(false);

        Assert.That(PartyExperienceBonus.GetMultiplier(general), Is.EqualTo(1.06).Within(0.0001));
        Assert.That(PartyExperienceBonus.IsSetParty(set), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(set), Is.EqualTo(1.06).Within(0.0001));
    }

    /// <summary>
    /// Tests that the bonus is taken from the configured attribute values, and not hard-coded.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask ConfiguredBonusIsUsedAsync()
    {
        var general = await this.CreateMembersAsync(false, this._darkWizard, this._darkWizard, this._darkWizard).ConfigureAwait(false);
        var set = await this.CreateMembersAsync(false, this._darkWizard, this._darkKnight, this._fairyElf).ConfigureAwait(false);
        general[0].Attributes!.AddElement(new SimpleElement(0.25f, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);
        set[0].Attributes!.AddElement(new SimpleElement(0.1f, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);
        set[0].Attributes!.AddElement(new SimpleElement(0.5f, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.GetMultiplier(general), Is.EqualTo(1.75).Within(0.0001));
        Assert.That(PartyExperienceBonus.GetMultiplier(set), Is.EqualTo(1.8).Within(0.0001));
    }

    /// <summary>
    /// Tests that a non-positive set party bonus is ignored and doesn't reduce the experience.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask NonPositiveSetPartyBonusIsIgnoredAsync()
    {
        var members = await this.CreateMembersAsync(false, this._darkWizard, this._darkKnight, this._fairyElf).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(0.01f, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);
        members[0].Attributes!.AddElement(new SimpleElement(-0.02f, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(1.03).Within(0.0001));
    }

    /// <summary>
    /// Tests that a negative party experience multiplier penalizes general parties.
    /// </summary>
    /// <param name="memberCount">The member count.</param>
    /// <param name="expectedMultiplier">The expected multiplier.</param>
    /// <returns>The async task.</returns>
    [TestCase(2, 0.9)]
    [TestCase(3, 0.85)]
    [TestCase(4, 0.8)]
    [TestCase(5, 0.75)]
    public async ValueTask NegativePartyExperienceMultiplierPenalizesGeneralPartyAsync(int memberCount, double expectedMultiplier)
    {
        var members = await this.CreateMembersAsync(false, Enumerable.Repeat(this._darkWizard, memberCount).ToArray()).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(-0.05f, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.False);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(expectedMultiplier).Within(0.0001));
    }

    /// <summary>
    /// Tests that a negative party experience multiplier penalizes set parties, while the set party bonus still applies.
    /// </summary>
    /// <param name="memberCount">The member count.</param>
    /// <param name="expectedMultiplier">The expected multiplier.</param>
    /// <returns>The async task.</returns>
    [TestCase(3, 0.87)]
    [TestCase(4, 0.82)]
    [TestCase(5, 0.77)]
    public async ValueTask NegativePartyExperienceMultiplierPenalizesSetPartyAsync(int memberCount, double expectedMultiplier)
    {
        var classes = new[] { this._darkWizard, this._darkKnight, this._fairyElf, this._magicGladiator, this._darkLord };
        var members = await this.CreateMembersAsync(false, classes.Take(memberCount).ToArray()).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(-0.05f, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);
        members[0].Attributes!.AddElement(new SimpleElement(0.02f, AggregateType.AddRaw), Stats.SetPartyExperienceBonus);

        Assert.That(PartyExperienceBonus.IsSetParty(members), Is.True);
        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(expectedMultiplier).Within(0.0001));
    }

    /// <summary>
    /// Tests that the multiplier becomes zero when the configured penalty cancels out the bonus completely.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async ValueTask StrongPenaltyResultsInZeroMultiplierAsync()
    {
        var members = await this.CreateMembersAsync(false, Enumerable.Repeat(this._darkWizard, 5).ToArray()).ConfigureAwait(false);
        members[0].Attributes!.AddElement(new SimpleElement(-0.2f, AggregateType.AddRaw), Stats.PartyExperienceMultiplier);

        Assert.That(PartyExperienceBonus.GetMultiplier(members), Is.EqualTo(0.0).Within(0.0001));
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
                AddDefaultBonuses(player);
            }

            result.Add(player);
        }

        return result;
    }

    private static void AddDefaultBonuses(Player player)
    {
        var bonuses = new (AttributeDefinition Attribute, float Value)[]
        {
            (Stats.PartyExperienceMultiplier, 0.01f),
            (Stats.SetPartyExperienceBonus, 0f),
        };

        foreach (var (attribute, value) in bonuses)
        {
            player.Attributes!.AddElement(new SimpleElement(value, AggregateType.AddRaw), attribute);
        }
    }
}
