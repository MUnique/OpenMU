// <copyright file="BuiltInItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The built-in item stat profiles: the item formulas as the game logic computes them,
/// written in the parts of an <see cref="ItemStatProfile"/>.
/// The initialization data and the configuration update create the profiles of a configuration from them.
/// </summary>
/// <remarks>
/// The names and parts are the same as in the game client's item stat file. Where the server computes differently
/// than the client, the values are the server's, e.g. a leadership formula and 4 strength per option level.
/// Only the profiles "armor", "book" and "shield" compute differently than the default; the others name the
/// level bonus tables their items use, or keep stored requirements of items which have no slot anyway.
/// The terms are for the attributes of the item power-ups which the item formulas compute today: the client's damage
/// is the minimum physical damage (which the maximum damage follows), its magic power the staff, sceptre and book rise,
/// its block rate the defense rate, and its defense the base defense, or the shield defense of the shields.
/// The client's magic defense has no attribute on the server, so the profiles leave it out.
/// </remarks>
internal static class BuiltInItemStatProfiles
{
    /// <summary>
    /// Gets the name of the profile of armor pieces (helms, armors, pants, gloves and boots).
    /// </summary>
    public static string Armor => "armor";

    /// <summary>
    /// Gets the name of the profile of the summoner's books.
    /// </summary>
    public static string Book => "book";

    /// <summary>
    /// Gets the name of the profile of the first wings.
    /// </summary>
    public static string FirstTierWing => "firstTierWing";

    /// <summary>
    /// Gets the name of the profile of the helpers (pets, rings, pendants and other items of the groups 13 and 14 with a level requirement).
    /// </summary>
    public static string Helper => "helper";

    /// <summary>
    /// Gets the name of the profile of the orbs.
    /// </summary>
    public static string Orb => "orb";

    /// <summary>
    /// Gets the name of the profile of the dark lord's sceptres.
    /// </summary>
    public static string Sceptre => "sceptre";

    /// <summary>
    /// Gets the name of the profile of the scrolls.
    /// </summary>
    public static string Scroll => "scroll";

    /// <summary>
    /// Gets the name of the profile of the second tier capes.
    /// </summary>
    public static string SecondTierCape => "secondTierCape";

    /// <summary>
    /// Gets the name of the profile of the second wings.
    /// </summary>
    public static string SecondTierWing => "secondTierWing";

    /// <summary>
    /// Gets the name of the profile of the shields.
    /// </summary>
    public static string Shield => "shield";

    /// <summary>
    /// Gets the name of the profile of the small wings.
    /// </summary>
    public static string SmallWing => "smallWing";

    /// <summary>
    /// Gets the name of the profile of the third wings and capes.
    /// </summary>
    public static string ThirdTierWing => "thirdTierWing";

    /// <summary>
    /// Gets the name of the profile of the weapons.
    /// </summary>
    public static string Weapon => "weapon";

    /// <summary>
    /// Gets the names of the built-in profiles, the default first.
    /// </summary>
    public static IEnumerable<string> Names => Definitions.Select(definition => definition.Name);

    private static IReadOnlyList<ProfileDefinition> Definitions { get; } =
    [
        new(
            ItemStatProfile.DefaultName,
            LevelPerItemLevel: 0,
            ExcellentLevelAdd: 0,
            StrengthPerOptionLevel: 4,
            [
                new(Stats.MinimumPhysBaseDmgByWeapon, TablePart.Of(nameof(LevelBonusNames.DamageIncreaseWeapons)), TermPart.Values(25, 5, 0), TermPart.Values(0, 5, 40)),
                MagicPower(Stats.StaffRise),
                MagicPower(Stats.ScepterRise),
                MagicPower(Stats.BookRise),
                new(Stats.DefenseRatePvm, TablePart.Of(nameof(LevelBonusNames.DefenseRateIncreaseShields)), TermPart.Values(25, 5, 0), TermPart.None),
                new(Stats.DefenseBase, TablePart.Of(nameof(LevelBonusNames.DefenseIncreaseArmors)), TermPart.Values(12, 4, 5), TermPart.Values(3, 2, 30)),
            ],
            [
                FormulaDefinition.Grows(Stats.TotalStrengthRequirementValue, 20, 3, 3),
                FormulaDefinition.Grows(Stats.TotalAgilityRequirementValue, 20, 3, 3),
                FormulaDefinition.Grows(Stats.TotalEnergyRequirementValue, 20, 3, 4),
                FormulaDefinition.Grows(Stats.TotalVitalityRequirementValue, 20, 3, 3),
                FormulaDefinition.Grows(Stats.TotalLeadershipRequirementValue, 20, 3, 3),
            ]),
        new(Armor, StrengthPerOptionLevel: 5),
        new(Book, Requirements: [FormulaDefinition.Grows(Stats.TotalEnergyRequirementValue, 20, 1, 3)]),
        new(FirstTierWing, Terms: [TermsDefinition.WithTable(Stats.DefenseBase, nameof(LevelBonusNames.DefenseBonus1stAnd2ndWings))]),
        new(Helper),
        new(Orb, Requirements: KeptStatRequirements()),
        new(Sceptre, Terms: [new(Stats.ScepterRise, perLevelAfterHalving: 0)]),
        new(Scroll, Requirements: KeptStatRequirements()),
        new(SecondTierCape, Terms: [TermsDefinition.WithTable(Stats.DefenseBase, nameof(LevelBonusNames.DefenseBonus1stAnd2ndWings))]),
        new(SecondTierWing, Terms: [TermsDefinition.WithTable(Stats.DefenseBase, nameof(LevelBonusNames.DefenseBonus1stAnd2ndWings))]),
        new(Shield, Terms: [new(Stats.DefenseShield, TablePart.Of(nameof(LevelBonusNames.DefenseIncreaseShields)), TermPart.None, TermPart.Values(20, 2, 0, afterLevelBonus: true))]),
        new(SmallWing, Terms: [TermsDefinition.WithTable(Stats.DefenseBase, nameof(LevelBonusNames.DefenseBonus1stAnd2ndWings))]),
        new(ThirdTierWing, Terms: [TermsDefinition.WithTable(Stats.DefenseBase, nameof(LevelBonusNames.DefenseBonus3rdWings))]),
        new(Weapon),
    ];

    /// <summary>
    /// Writes the built-in values of the profile with the name of the given profile into it.
    /// </summary>
    /// <param name="profile">The profile, with the <see cref="ItemStatProfile.Name"/> of a built-in profile and no terms and formulas yet.</param>
    /// <param name="createTerms">Creates new stat terms.</param>
    /// <param name="createFormula">Creates a new requirement formula.</param>
    /// <param name="findTable">Finds the level bonus table of the configuration by its key of the <see cref="LevelBonusNames"/>, or <c>null</c> if it doesn't have it.</param>
    /// <param name="findAttribute">Finds the attribute of the configuration.</param>
    /// <exception cref="ArgumentException">The profile has no name of a built-in profile.</exception>
    public static void Fill(
        ItemStatProfile profile,
        Func<ItemStatTerms> createTerms,
        Func<ItemRequirementFormula> createFormula,
        Func<string, ItemLevelBonusTable?> findTable,
        Func<AttributeDefinition, AttributeDefinition> findAttribute)
    {
        var definition = Definitions.FirstOrDefault(d => d.Name == profile.Name)
                         ?? throw new ArgumentException($"There is no built-in item stat profile named '{profile.Name}'.", nameof(profile));

        profile.LevelPerItemLevel = definition.LevelPerItemLevel;
        profile.ExcellentLevelAdd = definition.ExcellentLevelAdd;
        profile.StrengthPerOptionLevel = definition.StrengthPerOptionLevel;
        foreach (var termsDefinition in definition.Terms ?? [])
        {
            var terms = createTerms();
            terms.Attribute = findAttribute(termsDefinition.Attribute);
            terms.TableState = termsDefinition.Table.State;
            terms.Table = termsDefinition.Table.Table is { } table ? findTable(table) : null;
            termsDefinition.Excellent.WriteTo(terms, isAncient: false);
            termsDefinition.Ancient.WriteTo(terms, isAncient: true);
            terms.PerLevelAfterHalving = termsDefinition.PerLevelAfterHalving;
            profile.StatTerms.Add(terms);
        }

        foreach (var requirement in definition.Requirements ?? [])
        {
            var formula = createFormula();
            formula.Attribute = findAttribute(requirement.Attribute);
            formula.KeepBase = requirement.KeepBase;
            formula.Add = requirement.Add;
            formula.LevelWeight = requirement.LevelWeight;
            formula.Percent = requirement.Percent;
            profile.RequirementFormulas.Add(formula);
        }
    }

    /// <summary>
    /// The terms of a rise of the magic power, which the client computes for its one magic power.
    /// </summary>
    private static TermsDefinition MagicPower(AttributeDefinition rise) =>
        new(rise, TablePart.None, TermPart.Values(25, 5, 0), TermPart.Values(0, 2, 60), PerLevelAfterHalving: 2);

    private static IReadOnlyList<FormulaDefinition> KeptStatRequirements() =>
    [
        FormulaDefinition.KeptBase(Stats.TotalStrengthRequirementValue),
        FormulaDefinition.KeptBase(Stats.TotalAgilityRequirementValue),
        FormulaDefinition.KeptBase(Stats.TotalEnergyRequirementValue),
        FormulaDefinition.KeptBase(Stats.TotalVitalityRequirementValue),
    ];

    /// <summary>
    /// A built-in profile. A part without a value isn't written by it.
    /// </summary>
    private sealed record ProfileDefinition(
        string Name,
        int? LevelPerItemLevel = null,
        int? ExcellentLevelAdd = null,
        int? StrengthPerOptionLevel = null,
        IReadOnlyList<TermsDefinition>? Terms = null,
        IReadOnlyList<FormulaDefinition>? Requirements = null);

    /// <summary>
    /// The terms of an attribute of a built-in profile.
    /// </summary>
    private sealed record TermsDefinition(AttributeDefinition Attribute, TablePart Table, TermPart Excellent, TermPart Ancient, int? PerLevelAfterHalving = null)
    {
        public TermsDefinition(AttributeDefinition attribute, int? perLevelAfterHalving)
            : this(attribute, TablePart.NotWritten, TermPart.NotWritten, TermPart.NotWritten, perLevelAfterHalving)
        {
        }

        public static TermsDefinition WithTable(AttributeDefinition attribute, string table) => new(attribute, TablePart.Of(table), TermPart.NotWritten, TermPart.NotWritten);
    }

    /// <summary>
    /// The level bonus table part of an attribute.
    /// </summary>
    private sealed record TablePart(ItemStatTermState State, string? Table)
    {
        public static TablePart NotWritten { get; } = new(ItemStatTermState.NotWritten, null);

        public static TablePart None { get; } = new(ItemStatTermState.None, null);

        public static TablePart Of(string table) => new(ItemStatTermState.Values, table);
    }

    /// <summary>
    /// The excellent or ancient part of an attribute.
    /// </summary>
    private sealed record TermPart(ItemStatTermState State, int Times, int Add, int DropLevelDivisor, bool AfterLevelBonus)
    {
        public static TermPart NotWritten { get; } = new(ItemStatTermState.NotWritten, 0, 0, 0, false);

        public static TermPart None { get; } = new(ItemStatTermState.None, 0, 0, 0, false);

        public static TermPart Values(int times, int add, int dropLevelDivisor, bool afterLevelBonus = false) => new(ItemStatTermState.Values, times, add, dropLevelDivisor, afterLevelBonus);

        public void WriteTo(ItemStatTerms terms, bool isAncient)
        {
            if (isAncient)
            {
                terms.AncientState = this.State;
                terms.AncientTimes = this.Times;
                terms.AncientAdd = this.Add;
                terms.AncientDropLevelDivisor = this.DropLevelDivisor;
                terms.AncientAfterLevelBonus = this.AfterLevelBonus;
            }
            else
            {
                terms.ExcellentState = this.State;
                terms.ExcellentTimes = this.Times;
                terms.ExcellentAdd = this.Add;
                terms.ExcellentDropLevelDivisor = this.DropLevelDivisor;
            }
        }
    }

    /// <summary>
    /// A requirement formula of a built-in profile.
    /// </summary>
    private sealed record FormulaDefinition(AttributeDefinition Attribute, bool KeepBase, int Add, int LevelWeight, int Percent)
    {
        public static FormulaDefinition Grows(AttributeDefinition attribute, int add, int levelWeight, int percent) => new(attribute, false, add, levelWeight, percent);

        public static FormulaDefinition KeptBase(AttributeDefinition attribute) => new(attribute, true, 0, 0, 0);
    }
}
