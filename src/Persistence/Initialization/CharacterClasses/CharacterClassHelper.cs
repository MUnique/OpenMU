// <copyright file="CharacterClassHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Helper class related to <see cref="CharacterClass"/>.
    /// </summary>
    public static class CharacterClassHelper
    {
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
                yield return characterClasses.FirstOrDefault(c => c.Number == (int)CharacterClassNumber.FistMaster);
                if (ragefighterClass < 3)
                {
                    yield return characterClasses.FirstOrDefault(c => c.Number == (int)CharacterClassNumber.RageFighter);
                }
            }
        }
    }
}
