// <copyright file="CharacterClassHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Helper class related to <see cref="CharacterClass"/>.
/// </summary>
public static class CharacterClassHelper
{
    private static readonly (CharacterClasses Classes, CharacterClassNumber Number)[] NumberMapping =
    {
        (CharacterClasses.GrandMaster, CharacterClassNumber.GrandMaster),
        (CharacterClasses.SoulMaster, CharacterClassNumber.SoulMaster),
        (CharacterClasses.DarkWizard, CharacterClassNumber.DarkWizard),
        (CharacterClasses.BladeMaster, CharacterClassNumber.BladeMaster),
        (CharacterClasses.BladeKnight, CharacterClassNumber.BladeKnight),
        (CharacterClasses.DarkKnight, CharacterClassNumber.DarkKnight),
        (CharacterClasses.HighElf, CharacterClassNumber.HighElf),
        (CharacterClasses.MuseElf, CharacterClassNumber.MuseElf),
        (CharacterClasses.FairyElf, CharacterClassNumber.FairyElf),
        (CharacterClasses.DuelMaster, CharacterClassNumber.DuelMaster),
        (CharacterClasses.MagicGladiator, CharacterClassNumber.MagicGladiator),
        (CharacterClasses.LordEmperor, CharacterClassNumber.LordEmperor),
        (CharacterClasses.DarkLord, CharacterClassNumber.DarkLord),
        (CharacterClasses.DimensionMaster, CharacterClassNumber.DimensionMaster),
        (CharacterClasses.BloodySummoner, CharacterClassNumber.BloodySummoner),
        (CharacterClasses.Summoner, CharacterClassNumber.Summoner),
        (CharacterClasses.FistMaster, CharacterClassNumber.FistMaster),
        (CharacterClasses.RageFighter, CharacterClassNumber.RageFighter),
    };

    /// <summary>
    /// Determines the <see cref="CharacterClass"/>es, depending on the given class ranks which are provided by original configuration files.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration which contains the character classes.</param>
    /// <param name="wizardClass">Class rank of the wizard class.</param>
    /// <param name="knightClass">Class rank of the knight class.</param>
    /// <param name="elfClass">Class rank of the elf class.</param>
    /// <param name="magicGladiatorClass">Class rank of the magic gladiator class.</param>
    /// <param name="darkLordClass">Class rank of the dark lord class.</param>
    /// <param name="summonerClass">Class rank of the summoner class.</param>
    /// <param name="ragefighterClass">Class rank of the ragefighter class.</param>
    /// <returns>The corresponding <see cref="CharacterClass"/>es of the provided class ranks.</returns>
    [Obsolete("Use the overload with the CharacterClasses enum instead.")]
    public static IEnumerable<CharacterClass> DetermineCharacterClasses(this GameConfiguration gameConfiguration, int wizardClass, int knightClass, int elfClass, int magicGladiatorClass, int darkLordClass, int summonerClass, int ragefighterClass)
    {
        var characterClasses = gameConfiguration.CharacterClasses;
        IEnumerable<CharacterClass> DetermineClass(int classValue, CharacterClassNumber masterClass, CharacterClassNumber? secondClass, CharacterClassNumber firstClass)
        {
            if (classValue > 0)
            {
                if (characterClasses.FirstOrDefault(c => c.Number == (int)masterClass) is { } master)
                {
                    yield return master;
                }

                if (classValue < 3)
                {
                    if (secondClass is { } && characterClasses.FirstOrDefault(c => c.Number == (int)secondClass) is { } second)
                    {
                        yield return second;
                    }

                    if (characterClasses.FirstOrDefault(c => c.Number == (int)firstClass) is { } first)
                    {
                        yield return first;
                    }
                }
            }
        }

        var result = new List<CharacterClass>();
        result.AddRange(DetermineClass(wizardClass, CharacterClassNumber.GrandMaster, CharacterClassNumber.SoulMaster, CharacterClassNumber.DarkWizard));
        result.AddRange(DetermineClass(knightClass, CharacterClassNumber.BladeMaster, CharacterClassNumber.BladeKnight, CharacterClassNumber.DarkKnight));
        result.AddRange(DetermineClass(elfClass, CharacterClassNumber.HighElf, CharacterClassNumber.MuseElf, CharacterClassNumber.FairyElf));
        result.AddRange(DetermineClass(magicGladiatorClass, CharacterClassNumber.DuelMaster, null, CharacterClassNumber.MagicGladiator));
        result.AddRange(DetermineClass(darkLordClass, CharacterClassNumber.LordEmperor, null, CharacterClassNumber.DarkLord));
        result.AddRange(DetermineClass(summonerClass, CharacterClassNumber.DimensionMaster, CharacterClassNumber.BloodySummoner, CharacterClassNumber.Summoner));
        result.AddRange(DetermineClass(ragefighterClass, CharacterClassNumber.FistMaster, null, CharacterClassNumber.RageFighter));
        return result;
    }

    /// <summary>
    /// Determines the <see cref="CharacterClass" />es, depending on the given class ranks which are provided by original configuration files.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration which contains the character classes.</param>
    /// <param name="classes">The flags enum of character classes.</param>
    /// <param name="ignoreMissing">If set to <c>true</c>, missing classes are ignored and throw no exception.</param>
    /// <returns>
    /// The corresponding <see cref="CharacterClass" />es of the provided class ranks.
    /// </returns>
    public static IEnumerable<CharacterClass> DetermineCharacterClasses(this GameConfiguration gameConfiguration, CharacterClasses classes, bool ignoreMissing = false)
    {
        var characterClasses = gameConfiguration.CharacterClasses;
        foreach (var characterClassNumber in NumberMapping.Where(c => classes.HasFlag(c.Classes)).Select(c => c.Number))
        {
            yield return characterClasses.First(c => c.Number == (int)characterClassNumber);
        }
    }

    /// <summary>
    /// Determines the <see cref="CharacterClass"/>es, depending on the given classes which are provided by original configuration files.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration which contains the character classes.</param>
    /// <param name="wizardClass">Class of the wizard class.</param>
    /// <param name="knightClass">Class of the knight class.</param>
    /// <param name="elfClass">Class of the elf class.</param>
    /// <returns>The corresponding <see cref="CharacterClass"/>es of the provided class ranks.</returns>
    public static IEnumerable<CharacterClass> DetermineCharacterClasses(this GameConfiguration gameConfiguration, bool wizardClass, bool knightClass, bool elfClass)
    {
        var characterClasses = gameConfiguration.CharacterClasses;
        if (wizardClass)
        {
            yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.DarkWizard);
        }

        if (knightClass)
        {
            yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.DarkKnight);
        }

        if (elfClass)
        {
            yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.FairyElf);
        }
    }

    /// <summary>
    /// Creates the attribute relationship.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <param name="sourceAttribute">The source attribute.</param>
    /// <param name="inputOperator">The input operator.</param>
    /// <param name="aggregateType">The aggregate type with which the relationship will effect the <paramref name="targetAttribute"/>.</param>
    /// <returns>The attribute relationship.</returns>
    public static AttributeRelationship CreateAttributeRelationship(IContext context, GameConfiguration gameConfiguration, AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply, AggregateType aggregateType = AggregateType.AddRaw)
    {
        return context.CreateNew<AttributeRelationship>(
            targetAttribute.GetPersistent(gameConfiguration) ?? targetAttribute,
            multiplier,
            sourceAttribute.GetPersistent(gameConfiguration) ?? sourceAttribute,
            inputOperator,
            default(AttributeDefinition?),
            aggregateType);
    }

    /// <summary>
    /// Creates the attribute relationship.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="multiplierAttribute">The multiplier attribute.</param>
    /// <param name="sourceAttribute">The source attribute.</param>
    /// <param name="inputOperator">The input operator.</param>
    /// <param name="aggregateType">The aggregate type with which the relationship will effect the <paramref name="targetAttribute"/>.</param>
    /// <returns>The attribute relationship.</returns>
    public static AttributeRelationship CreateAttributeRelationship(IContext context, GameConfiguration gameConfiguration, AttributeDefinition targetAttribute, AttributeDefinition multiplierAttribute, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply, AggregateType aggregateType = AggregateType.AddRaw)
    {
        return context.CreateNew<AttributeRelationship>(
            targetAttribute.GetPersistent(gameConfiguration) ?? targetAttribute,
            0f,
            sourceAttribute.GetPersistent(gameConfiguration) ?? sourceAttribute,
            inputOperator,
            multiplierAttribute.GetPersistent(gameConfiguration) ?? multiplierAttribute,
            aggregateType);
    }

    /// <summary>
    /// Creates the conditional relationship between attributes.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="conditionalAttribute">The conditional attribute.</param>
    /// <param name="sourceAttribute">The source attribute.</param>
    /// <param name="aggregateType">The aggregate type with which the relationship will effect the <paramref name="targetAttribute"/>.</param>
    /// <returns>The attribute relationship.</returns>
    public static AttributeRelationship CreateConditionalRelationship(IContext context, GameConfiguration gameConfiguration, AttributeDefinition targetAttribute, AttributeDefinition conditionalAttribute, AttributeDefinition sourceAttribute, AggregateType aggregateType = AggregateType.AddRaw)
    {
        return context.CreateNew<AttributeRelationship>(
            targetAttribute.GetPersistent(gameConfiguration) ?? targetAttribute,
            conditionalAttribute.GetPersistent(gameConfiguration) ?? conditionalAttribute,
            sourceAttribute.GetPersistent(gameConfiguration) ?? sourceAttribute,
            aggregateType);
    }

    /// <summary>
    /// Creates the constant value attribute.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="value">The value.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns>The constant value attribute.</returns>
    public static ConstValueAttribute CreateConstValueAttribute(IContext context, GameConfiguration gameConfiguration, float value, AttributeDefinition attribute)
    {
        return context.CreateNew<ConstValueAttribute>(value, attribute.GetPersistent(gameConfiguration));
    }
}