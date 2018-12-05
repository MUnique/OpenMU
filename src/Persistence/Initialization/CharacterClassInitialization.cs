// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System;
    using System.Linq;

    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization of character classes data.
    /// </summary>
    internal class CharacterClassInitialization
    {
        private const int LorenciaMapId = 0;
        private const int NoriaMapId = 3;

        private readonly IContext context;
        private readonly GameConfiguration gameConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterClassInitialization" /> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public CharacterClassInitialization(IContext context, GameConfiguration gameConfiguration)
        {
            this.context = context;
            this.gameConfiguration = gameConfiguration;
        }

        /// <summary>
        /// Creates the character classes.
        /// </summary>
        public void CreateCharacterClasses()
        {
            var battleMaster = this.CreateBattleMaster();
            var bladeKnight = this.CreateBladeKnight(battleMaster);
            this.CreateDarkKnight(bladeKnight);

            var grandMaster = this.CreateGrandMaster();
            var soulMaster = this.CreateSoulMaster(grandMaster);
            this.CreateDarkWizard(soulMaster);

            var highElf = this.CreateHighElf();
            var museElf = this.CreateMuseElf(highElf);
            this.CreateFairyElf(museElf);

            var duelMaster = this.CreateDuelMaster();
            this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, duelMaster, true);

            var lordEmperor = this.CreateLordEmperor();
            this.CreateDarkLord(CharacterClassNumber.DarkLord, "Dark Lord", false, lordEmperor, true);
        }

        private CharacterClass CreateLordEmperor()
        {
            var result = this.CreateDarkLord(CharacterClassNumber.LordEmperor, "Lord Emperor", true, null, false);
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
            return result;
        }

        private CharacterClass CreateDarkLord(CharacterClassNumber number, string name, bool isMaster, CharacterClass nextGenerationClass, bool canGetCreated)
        {
            var energyMinus15 = this.context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "TotalEnergy minus 15", "TotalEnergy minus 15");
            this.gameConfiguration.Attributes.Add(energyMinus15);

            var result = this.context.CreateNew<CharacterClass>();
            this.gameConfiguration.CharacterClasses.Add(result);
            result.CanGetCreated = canGetCreated;
            result.HomeMap = this.gameConfiguration.Maps.FirstOrDefault(map => map.Number == LorenciaMapId);
            result.Number = (byte)number;
            result.Name = name;
            result.PointsPerLevelUp = 7;
            result.LevelRequirementByCreation = 250;
            result.IsMasterClass = isMaster;
            result.NextGenerationClass = nextGenerationClass;
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 26, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 20, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 20, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 15, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseLeadership, 25, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 7, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 1.0f / 7, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 2.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.0f / 6, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.0f / 10, Stats.TotalLeadership));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 4.0f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.15f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.1f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalLeadership));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalLeadership));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(energyMinus15, -15, Stats.TotalEnergy, InputOperator.Add));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.5f, energyMinus15));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 0.5f, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 3, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 7, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 5, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 14, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 10, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.SkillMultiplier, 0.005f, Stats.TotalEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.TotalLeadership));
            /* TODO: Add these stats
                                        Critical dmg = cmd/25+str/30
                                        Fireburst bonus min dmg = 100+str/25+ene/50
                                        Fireburst bonus max dmg = 150+str/25+ene/50
                                        Horse bonus dmg = 100+horseLvl*10+lvl*2.5+str/10+cmd/5
                                        Raven speed = 20+(ravenLvl*4)/5+cmd/50
                                        Raven min dmg = 180+ravenLvl*15+cmd/8
                                        Raven max dmg = 200+ravenLvl*15+cmd/4
                                    */

            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(38, Stats.MaximumMana));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(48.5f, Stats.MaximumHealth));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.SkillMultiplier));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 27.5f, Stats.ManaRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MoneyAmountRate));

            return result;
        }

        private CharacterClass CreateDuelMaster()
        {
            var result = this.CreateMagicGladiator(CharacterClassNumber.DuelMaster, "Duel Master", true, null, false);
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
            return result;
        }

        private CharacterClass CreateMagicGladiator(CharacterClassNumber number, string name, bool isMaster, CharacterClass nextGenerationClass, bool canGetCreated)
        {
            var result = this.context.CreateNew<CharacterClass>();
            this.gameConfiguration.CharacterClasses.Add(result);
            result.CanGetCreated = canGetCreated;
            result.HomeMap = this.gameConfiguration.Maps.FirstOrDefault(map => map.Number == LorenciaMapId);
            result.Number = (byte)number;
            result.Name = name;
            result.PointsPerLevelUp = 7;
            result.IsMasterClass = isMaster;
            result.LevelRequirementByCreation = 220;
            result.NextGenerationClass = nextGenerationClass;
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 26, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 26, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 26, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 16, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 5, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 1.0f / 3, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.25f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.15f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.25f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 2, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 6, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 4, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 12, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 8, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1.0f / 9, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1.0f / 4, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));

            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(57, Stats.MaximumHealth));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(7, Stats.MaximumMana));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.SkillMultiplier));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 27.5f, Stats.ManaRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MoneyAmountRate));
            return result;
        }

        private void CreateFairyElf(CharacterClass museElf)
        {
            this.CreateElf(CharacterClassNumber.FairyElf, "Fairy Elf", 5, false, museElf, true);
        }

        private CharacterClass CreateMuseElf(CharacterClass highElf)
        {
            return this.CreateElf(CharacterClassNumber.MuseElf, "Muse Elf", 6, false, highElf, false);
        }

        private CharacterClass CreateHighElf()
        {
            var result = this.CreateElf(CharacterClassNumber.HighElf, "High Elf", 6, true, null, false);
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
            return result;
        }

        private CharacterClass CreateElf(CharacterClassNumber number, string name, short pointsPerLevelUp, bool isMaster, CharacterClass nextGenerationClass, bool canGetCreated)
        {
            var result = this.context.CreateNew<CharacterClass>();
            this.gameConfiguration.CharacterClasses.Add(result);
            result.CanGetCreated = canGetCreated;
            result.HomeMap = this.gameConfiguration.Maps.FirstOrDefault(map => map.Number == NoriaMapId);
            result.Number = (byte)number;
            result.Name = name;
            result.PointsPerLevelUp = pointsPerLevelUp;
            result.IsMasterClass = isMaster;
            result.NextGenerationClass = nextGenerationClass;
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 22, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 25, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 20, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 15, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 80, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 30, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 10, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 0.25f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.1f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 0.6f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.5f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.5f, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 7, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 4, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 14, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 8, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));

            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(39, Stats.MaximumHealth));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(6, Stats.MaximumMana));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.SkillMultiplier));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 27.5f, Stats.ManaRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MoneyAmountRate));
            return result;
        }

        private void CreateDarkWizard(CharacterClass soulMaster)
        {
            this.CreateWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", 5, false, soulMaster, true);
        }

        private CharacterClass CreateSoulMaster(CharacterClass grandMaster)
        {
            return this.CreateWizard(CharacterClassNumber.SoulMaster, "Soul Master", 6, false, grandMaster, false);
        }

        private CharacterClass CreateGrandMaster()
        {
            var result = this.CreateWizard(CharacterClassNumber.GrandMaster, "Grand Master", 6, true, null, false);
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
            return result;
        }

        private CharacterClass CreateWizard(CharacterClassNumber number, string name, short pointsPerLevelUp, bool isMaster, CharacterClass nextGenerationClass, bool canGetCreated)
        {
            var result = this.context.CreateNew<CharacterClass>();
            this.gameConfiguration.CharacterClasses.Add(result);
            result.CanGetCreated = canGetCreated;
            result.HomeMap = this.gameConfiguration.Maps.FirstOrDefault(map => map.Number == LorenciaMapId);
            result.Number = (byte)number;
            result.Name = name;
            result.PointsPerLevelUp = pointsPerLevelUp;
            result.IsMasterClass = isMaster;
            result.NextGenerationClass = nextGenerationClass;
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 18, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 18, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 15, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 30, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 4, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 1.0f / 3, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.25f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 4, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.4f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 2, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 2, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 6, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 4, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1.0f / 9, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1.0f / 4, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));

            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(30, Stats.MaximumHealth));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.SkillMultiplier));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 27.5f, Stats.ManaRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MoneyAmountRate));

            return result;
        }

        private CharacterClass CreateBattleMaster()
        {
            var result = this.CreateKnight(CharacterClassNumber.BattleMaster, "Battle Master", 6, true, null, false);
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
            return result;
        }

        private CharacterClass CreateBladeKnight(CharacterClass battleMaster)
        {
            return this.CreateKnight(CharacterClassNumber.BladeKnight, "Blade Knight", 6, false, battleMaster, false);
        }

        private void CreateDarkKnight(CharacterClass bladeKnight)
        {
            this.CreateKnight(CharacterClassNumber.DarkKnight, "Dark Knight", 5, false, bladeKnight, true);
        }

        private CharacterClass CreateKnight(CharacterClassNumber number, string name, short pointsPerLevelUp, bool isMaster, CharacterClass nextGenerationClass, bool canGetCreated)
        {
            var result = this.context.CreateNew<CharacterClass>();
            this.gameConfiguration.CharacterClasses.Add(result);
            result.CanGetCreated = canGetCreated;
            result.HomeMap = this.gameConfiguration.Maps.FirstOrDefault(map => map.Number == LorenciaMapId);
            result.Number = (byte)number;
            result.Name = name;
            result.PointsPerLevelUp = pointsPerLevelUp;
            result.IsMasterClass = isMaster;
            result.NextGenerationClass = nextGenerationClass;
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 28, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 20, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 25, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 10, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 3, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 1.0f / 3, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 4.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 1, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.15f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 0.5f, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 3, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 6, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 4, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.SkillMultiplier, 0.01f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));

            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(10, Stats.MaximumMana));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(35, Stats.MaximumHealth));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.SkillMultiplier));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(0.05f, Stats.AbilityRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 27.5f, Stats.ManaRecovery));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MoneyAmountRate));
            return result;
        }

        private StatAttributeDefinition CreateStatAttributeDefinition(AttributeDefinition attribute, int value, bool increasableByPlayer)
        {
            var definition = this.context.CreateNew<StatAttributeDefinition>(attribute.GetPersistent(this.gameConfiguration), value, increasableByPlayer);
            return definition;
        }

        private AttributeRelationship CreateAttributeRelationship(AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply)
        {
            var relationship = this.context.CreateNew<AttributeRelationship>(targetAttribute.GetPersistent(this.gameConfiguration) ?? targetAttribute, multiplier, sourceAttribute.GetPersistent(this.gameConfiguration) ?? sourceAttribute, inputOperator);
            return relationship;
        }

        private ConstValueAttribute CreateConstValueAttribute(float value, AttributeDefinition attribute)
        {
            return this.context.CreateNew<ConstValueAttribute>(value, attribute.GetPersistent(this.gameConfiguration));
        }
    }
}
