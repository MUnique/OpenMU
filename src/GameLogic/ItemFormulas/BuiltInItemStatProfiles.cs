// <copyright file="BuiltInItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.ItemFormulas;

using MUnique.OpenMU.DataModel.Configuration.Items;

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
/// </remarks>
public static partial class BuiltInItemStatProfiles
{
    /// <summary>
    /// The profile of armor pieces (helms, armors, pants, gloves and boots).
    /// </summary>
    public const string Armor = "armor";

    /// <summary>
    /// The profile of the summoner's books.
    /// </summary>
    public const string Book = "book";

    /// <summary>
    /// The profile of the first wings.
    /// </summary>
    public const string FirstTierWing = "firstTierWing";

    /// <summary>
    /// The profile of the helpers (pets, rings, pendants and other items of the groups 13 and 14 with a level requirement).
    /// </summary>
    public const string Helper = "helper";

    /// <summary>
    /// The profile of the orbs.
    /// </summary>
    public const string Orb = "orb";

    /// <summary>
    /// The profile of the dark lord's sceptres.
    /// </summary>
    public const string Sceptre = "sceptre";

    /// <summary>
    /// The profile of the scrolls.
    /// </summary>
    public const string Scroll = "scroll";

    /// <summary>
    /// The profile of the second tier capes.
    /// </summary>
    public const string SecondTierCape = "secondTierCape";

    /// <summary>
    /// The profile of the second wings.
    /// </summary>
    public const string SecondTierWing = "secondTierWing";

    /// <summary>
    /// The profile of the shields.
    /// </summary>
    public const string Shield = "shield";

    /// <summary>
    /// The profile of the small wings.
    /// </summary>
    public const string SmallWing = "smallWing";

    /// <summary>
    /// The profile of the third wings and capes.
    /// </summary>
    public const string ThirdTierWing = "thirdTierWing";

    /// <summary>
    /// The profile of the weapons.
    /// </summary>
    public const string Weapon = "weapon";

    private static readonly IReadOnlyList<ProfileDefinition> Definitions =
    [
        new(
            ItemStatProfile.DefaultName,
            LevelPerItemLevel: 0,
            ExcellentLevelAdd: 0,
            StrengthPerOptionLevel: 4,
            [
                new(ItemStat.Damage, TablePart.Of(BuiltInLevelBonusTable.DamageIncreaseWeapons), TermPart.Values(25, 5, 0), TermPart.Values(0, 5, 40)),
                new(ItemStat.MagicPower, TablePart.None, TermPart.Values(25, 5, 0), TermPart.Values(0, 2, 60), PerLevelAfterHalving: 2),
                new(ItemStat.BlockRate, TablePart.Of(BuiltInLevelBonusTable.DefenseRateIncreaseShields), TermPart.Values(25, 5, 0), TermPart.None),
                new(ItemStat.Defense, TablePart.Of(BuiltInLevelBonusTable.DefenseIncreaseArmors), TermPart.Values(12, 4, 5), TermPart.Values(3, 2, 30)),
                new(ItemStat.MagicDefense, TablePart.None, TermPart.None, TermPart.None),
            ],
            [
                FormulaDefinition.Grows(ItemRequirementStat.Strength, 20, 3, 3),
                FormulaDefinition.Grows(ItemRequirementStat.Agility, 20, 3, 3),
                FormulaDefinition.Grows(ItemRequirementStat.Energy, 20, 3, 4),
                FormulaDefinition.Grows(ItemRequirementStat.Vitality, 20, 3, 3),
                FormulaDefinition.Grows(ItemRequirementStat.Leadership, 20, 3, 3),
            ]),
        new(Armor, StrengthPerOptionLevel: 5),
        new(Book, Requirements: [FormulaDefinition.Grows(ItemRequirementStat.Energy, 20, 1, 3)]),
        new(FirstTierWing, Stats: [StatDefinition.WithTable(ItemStat.Defense, BuiltInLevelBonusTable.DefenseBonus1stAnd2ndWings)]),
        new(Helper),
        new(Orb, Requirements: KeptStatRequirements()),
        new(Sceptre, Stats: [new(ItemStat.MagicPower, perLevelAfterHalving: 0)]),
        new(Scroll, Requirements: KeptStatRequirements()),
        new(SecondTierCape, Stats: [StatDefinition.WithTable(ItemStat.Defense, BuiltInLevelBonusTable.DefenseBonus1stAnd2ndWings)]),
        new(SecondTierWing, Stats: [StatDefinition.WithTable(ItemStat.Defense, BuiltInLevelBonusTable.DefenseBonus1stAnd2ndWings)]),
        new(Shield, Stats: [new(ItemStat.Defense, TablePart.Of(BuiltInLevelBonusTable.DefenseIncreaseShields), TermPart.None, TermPart.Values(20, 2, 0, afterLevelBonus: true))]),
        new(SmallWing, Stats: [StatDefinition.WithTable(ItemStat.Defense, BuiltInLevelBonusTable.DefenseBonus1stAnd2ndWings)]),
        new(ThirdTierWing, Stats: [StatDefinition.WithTable(ItemStat.Defense, BuiltInLevelBonusTable.DefenseBonus3rdWings)]),
        new(Weapon),
    ];

    /// <summary>
    /// Gets the names of the built-in profiles, the default first.
    /// </summary>
    public static IEnumerable<string> Names => Definitions.Select(definition => definition.Name);

    /// <summary>
    /// Writes the built-in values of the profile with the name of the given profile into it.
    /// </summary>
    /// <param name="profile">The profile, with the <see cref="ItemStatProfile.Name"/> of a built-in profile and no terms and formulas yet.</param>
    /// <param name="createTerms">Creates new stat terms.</param>
    /// <param name="createFormula">Creates a new requirement formula.</param>
    /// <param name="findTable">Finds the level bonus table of the configuration, or <c>null</c> if it doesn't have it.</param>
    /// <exception cref="ArgumentException">The profile has no name of a built-in profile.</exception>
    public static void Fill(
        ItemStatProfile profile,
        Func<ItemStatTerms> createTerms,
        Func<ItemRequirementFormula> createFormula,
        Func<BuiltInLevelBonusTable, ItemLevelBonusTable?> findTable)
    {
        var definition = Definitions.FirstOrDefault(d => d.Name == profile.Name)
                         ?? throw new ArgumentException($"There is no built-in item stat profile named '{profile.Name}'.", nameof(profile));

        profile.LevelPerItemLevel = definition.LevelPerItemLevel;
        profile.ExcellentLevelAdd = definition.ExcellentLevelAdd;
        profile.StrengthPerOptionLevel = definition.StrengthPerOptionLevel;
        foreach (var stat in definition.Stats ?? [])
        {
            var terms = createTerms();
            terms.Stat = stat.Stat;
            terms.TableState = stat.Table.State;
            terms.Table = stat.Table.Table is { } table ? findTable(table) : null;
            stat.Excellent.WriteTo(terms, isAncient: false);
            stat.Ancient.WriteTo(terms, isAncient: true);
            terms.PerLevelAfterHalving = stat.PerLevelAfterHalving;
            profile.StatTerms.Add(terms);
        }

        foreach (var requirement in definition.Requirements ?? [])
        {
            var formula = createFormula();
            formula.Stat = requirement.Stat;
            formula.KeepBase = requirement.KeepBase;
            formula.Add = requirement.Add;
            formula.LevelWeight = requirement.LevelWeight;
            formula.Percent = requirement.Percent;
            profile.RequirementFormulas.Add(formula);
        }
    }

    private static IReadOnlyList<FormulaDefinition> KeptStatRequirements() =>
    [
        FormulaDefinition.KeptBase(ItemRequirementStat.Strength),
        FormulaDefinition.KeptBase(ItemRequirementStat.Agility),
        FormulaDefinition.KeptBase(ItemRequirementStat.Energy),
        FormulaDefinition.KeptBase(ItemRequirementStat.Vitality),
    ];

    /// <summary>
    /// A built-in profile. A part without a value isn't written by it.
    /// </summary>
    private sealed record ProfileDefinition(
        string Name,
        int? LevelPerItemLevel = null,
        int? ExcellentLevelAdd = null,
        int? StrengthPerOptionLevel = null,
        IReadOnlyList<StatDefinition>? Stats = null,
        IReadOnlyList<FormulaDefinition>? Requirements = null);

    /// <summary>
    /// The terms of a stat of a built-in profile.
    /// </summary>
    private sealed record StatDefinition(ItemStat Stat, TablePart Table, TermPart Excellent, TermPart Ancient, int? PerLevelAfterHalving = null)
    {
        public StatDefinition(ItemStat stat, int? perLevelAfterHalving)
            : this(stat, TablePart.NotWritten, TermPart.NotWritten, TermPart.NotWritten, perLevelAfterHalving)
        {
        }

        public static StatDefinition WithTable(ItemStat stat, BuiltInLevelBonusTable table) => new(stat, TablePart.Of(table), TermPart.NotWritten, TermPart.NotWritten);
    }

    /// <summary>
    /// The level bonus table part of a stat.
    /// </summary>
    private sealed record TablePart(ItemStatTermState State, BuiltInLevelBonusTable? Table)
    {
        public static TablePart NotWritten { get; } = new(ItemStatTermState.NotWritten, null);

        public static TablePart None { get; } = new(ItemStatTermState.None, null);

        public static TablePart Of(BuiltInLevelBonusTable table) => new(ItemStatTermState.Values, table);
    }

    /// <summary>
    /// The excellent or ancient part of a stat.
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
    private sealed record FormulaDefinition(ItemRequirementStat Stat, bool KeepBase, int Add, int LevelWeight, int Percent)
    {
        public static FormulaDefinition Grows(ItemRequirementStat stat, int add, int levelWeight, int percent) => new(stat, false, add, levelWeight, percent);

        public static FormulaDefinition KeptBase(ItemRequirementStat stat) => new(stat, true, 0, 0, 0);
    }
}
