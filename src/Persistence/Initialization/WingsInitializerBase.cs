// <copyright file="WingsInitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Base class for wing data initializing.
/// </summary>
public abstract class WingsInitializerBase : InitializerBase
{
    private static readonly float[] DefenseIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };
    private static readonly float[] DefenseIncreaseByLevelThirdWings = { 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 41, 47, 54, 62, 71, 81 };

    /// <summary>
    /// Initializes a new instance of the <see cref="WingsInitializerBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected WingsInitializerBase(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    protected enum OptionType
    {
        HealthRecover,
        PhysDamage,
        WizDamage,
        CurseDamage,
        Defense,
    }

    protected abstract int MaximumItemLevel { get; }

    /// <summary>
    /// Builds the options based on the given parameters.
    /// </summary>
    /// <remarks>
    /// Some wings can possibly have different item options of <see cref="ItemOptionTypes.Option"/>, depending on the outcome of consumption of the 'Jewel of Life'.
    /// Since webzen did a "great" job defining different numbers (representations in their transmitted item data) for the same options,
    /// we have to build <see cref="IncreasableItemOption"/>s for each item separately.
    /// We don't want to handle this stuff per-item in our <see cref="ItemSerializer"/> since we want a generic solution.
    /// </remarks>
    /// <param name="optionsWithNumbers">The tuples of option type with their number.</param>
    /// <returns>The built <see cref="IncreasableItemOption"/>s.</returns>
    protected IEnumerable<IncreasableItemOption> BuildOptions(params (int, OptionType)[] optionsWithNumbers)
    {
        foreach (var tuple in optionsWithNumbers)
        {
            switch (tuple.Item2)
            {
                case OptionType.CurseDamage:
                    yield return this.CreateItemOption(tuple.Item1, Stats.MaximumCurseBaseDmg, 0, AggregateType.AddRaw, 4f, ItemOptionDefinitionNumbers.WingCurse);
                    break;
                case OptionType.Defense:
                    yield return this.CreateItemOption(tuple.Item1, Stats.DefenseBase, 0, AggregateType.AddRaw, 4f, ItemOptionDefinitionNumbers.WingDefense);
                    break;
                case OptionType.HealthRecover:
                    yield return this.CreateItemOption(tuple.Item1, Stats.HealthRecoveryMultiplier, 0, AggregateType.AddRaw, 0.01f, ItemOptionDefinitionNumbers.WingHealthRecover);
                    break;
                case OptionType.PhysDamage:
                    yield return this.CreateItemOption(tuple.Item1, Stats.MaximumPhysBaseDmg, 0, AggregateType.AddRaw, 4f, ItemOptionDefinitionNumbers.WingPhysical);
                    break;
                case OptionType.WizDamage:
                    yield return this.CreateItemOption(tuple.Item1, Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, 4f, ItemOptionDefinitionNumbers.WingWizardry);
                    break;
                default:
                    throw new ArgumentException("unknown OptionType");
            }
        }
    }

    protected ItemLevelBonusTable CreateAbsorbBonusPerLevel()
    {
        IEnumerable<float> Generate()
        {
            yield return 1f;
            for (int level = 1; level <= this.MaximumItemLevel; level++)
            {
                yield return 1f - (0.02f * level);
            }
        }

        return this.CreateItemBonusTable(Generate().ToArray(), "Wing absorb", "The damage absorb of wings per item level, 2 % less damage per level.");
    }

    protected ItemLevelBonusTable CreateDamageIncreaseBonusPerLevelFirstAndThirdWings()
    {
        IEnumerable<float> Generate()
        {
            yield return 1f;
            for (int level = 1; level <= this.MaximumItemLevel; level++)
            {
                yield return 1f + (0.02f * level);
            }
        }

        return this.CreateItemBonusTable(Generate().ToArray(), "Damage Increase (1st and 3rd Wings)", "Defines the damage increase multiplier for first and third level wings. It's 2 % per wing level.");
    }

    protected ItemLevelBonusTable CreateDamageIncreaseBonusPerLevelSecondWings()
    {
        IEnumerable<float> Generate()
        {
            yield return 1;
            for (int level = 1; level <= this.MaximumItemLevel; level++)
            {
                yield return 1 + (0.01f * level);
            }
        }

        return this.CreateItemBonusTable(Generate().ToArray(), "Damage Increase (2nd Wings)", "Defines the damage increase multiplier for second level wings. It's 1 % per wing level.");
    }

    protected ItemLevelBonusTable CreateBonusDefensePerLevel()
    {
        return this.CreateItemBonusTable(DefenseIncreaseByLevel, "Defense Bonus (1st and 2nd Wings)", "Defines the defense bonus per level for 1st and 2nd level wings.");
    }

    protected ItemLevelBonusTable CreateBonusDefensePerLevelThirdWings()
    {
        return this.CreateItemBonusTable(DefenseIncreaseByLevelThirdWings, "Defense Bonus (3rd Wings)", "Defines the defense bonus per level for 3rd level wings.");
    }
}