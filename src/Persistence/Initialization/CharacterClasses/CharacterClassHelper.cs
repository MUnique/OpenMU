// <copyright file="CharacterClassHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            if (wizardClass > 0)
            {
                yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.GrandMaster);

                if (wizardClass < 3)
                {
                    yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.SoulMaster);
                    if (wizardClass < 2)
                    {
                        yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.DarkWizard);
                    }
                }
            }

            if (knightClass > 0)
            {
                yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.BladeMaster);
                if (knightClass < 3)
                {
                    yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.BladeKnight);
                    if (knightClass < 2)
                    {
                        yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.DarkKnight);
                    }
                }
            }

            if (elfClass > 0)
            {
                yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.HighElf);
                if (elfClass < 3)
                {
                    yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.MuseElf);
                    if (elfClass < 2)
                    {
                        yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.FairyElf);
                    }
                }
            }

            if (magicGladiatorClass > 0)
            {
                yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.DuelMaster);
                if (magicGladiatorClass < 3)
                {
                    yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.MagicGladiator);
                }
            }

            if (darkLordClass > 0)
            {
                yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.LordEmperor);
                if (darkLordClass < 3)
                {
                    yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.DarkLord);
                }
            }

            if (summonerClass > 0)
            {
                yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.DimensionMaster);
                if (summonerClass < 3)
                {
                    yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.BloodySummoner);
                    if (summonerClass < 2)
                    {
                        yield return characterClasses.First(c => c.Number == (int)CharacterClassNumber.Summoner);
                    }
                }
            }

            if (ragefighterClass > 0)
            {
                if (characterClasses.FirstOrDefault(c => c.Number == (int)CharacterClassNumber.FistMaster) is { } fistMaster)
                {
                    yield return fistMaster;
                }

                if (ragefighterClass < 3)
                {
                    if (characterClasses.FirstOrDefault(c => c.Number == (int)CharacterClassNumber.RageFighter) is { } rageFighter)
                    {
                        yield return rageFighter;
                    }
                }
            }
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
    }
}
