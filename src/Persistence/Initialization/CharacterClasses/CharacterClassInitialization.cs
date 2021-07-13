// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization of character classes data.
    /// </summary>
    internal partial class CharacterClassInitialization : InitializerBase
    {
        private const int LorenciaMapId = 0;
        private const int NoriaMapId = 3;
        private const int ElvenlandMapId = 51;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterClassInitialization" /> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public CharacterClassInitialization(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <summary>
        /// Gets a value indicating whether to use classic PVP, which uses no shield stats and the same attack/defense rate as PvM.
        /// </summary>
        protected virtual bool UseClassicPvp { get; } = false;

        /// <summary>
        /// Creates the character classes.
        /// </summary>
        public override void Initialize()
        {
            var bladeMaster = this.CreateBladeMaster();
            var bladeKnight = this.CreateBladeKnight(bladeMaster);
            this.CreateDarkKnight(CharacterClassNumber.DarkKnight, "Dark Knight", false, bladeKnight, true);

            var grandMaster = this.CreateGrandMaster();
            var soulMaster = this.CreateSoulMaster(grandMaster);
            this.CreateDarkWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", false, soulMaster, true);

            var highElf = this.CreateHighElf();
            var museElf = this.CreateMuseElf(highElf);
            this.CreateFairyElf(CharacterClassNumber.FairyElf, "Fairy Elf", false, museElf, true);

            var dimensionMaster = this.CreateDimensionMaster();
            var bloodySummoner = this.CreateBloodySummoner(dimensionMaster);
            this.CreateSummoner(CharacterClassNumber.Summoner, "Summoner", false, bloodySummoner, true);

            var duelMaster = this.CreateDuelMaster();
            this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, duelMaster, true);

            var lordEmperor = this.CreateLordEmperor();
            this.CreateDarkLord(CharacterClassNumber.DarkLord, "Dark Lord", false, lordEmperor, true);

            var fistMaster = this.CreateFistMaster();
            this.CreateRageFighter(CharacterClassNumber.RageFighter, "Rage Fighter", false, fistMaster, true);
        }

        private StatAttributeDefinition CreateStatAttributeDefinition(AttributeDefinition attribute, int value, bool increasableByPlayer)
        {
            var definition = this.Context.CreateNew<StatAttributeDefinition>(attribute.GetPersistent(this.GameConfiguration), value, increasableByPlayer);
            return definition;
        }

        private AttributeRelationship CreateAttributeRelationship(AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply)
        {
            var relationship = this.Context.CreateNew<AttributeRelationship>(targetAttribute.GetPersistent(this.GameConfiguration) ?? targetAttribute, multiplier, sourceAttribute.GetPersistent(this.GameConfiguration) ?? sourceAttribute, inputOperator);
            return relationship;
        }

        private ConstValueAttribute CreateConstValueAttribute(float value, AttributeDefinition attribute)
        {
            return this.Context.CreateNew<ConstValueAttribute>(value, attribute.GetPersistent(this.GameConfiguration));
        }

        private void AddCommonBaseAttributeValues(ICollection<ConstValueAttribute> baseAttributeValues, bool isMaster)
        {
            baseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 27.5f, Stats.ManaRecoveryMultiplier));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.TwoHandedWeaponDamageIncrease));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MoneyAmountRate));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.ExperienceRate));
            baseAttributeValues.Add(this.CreateConstValueAttribute(0.03f, Stats.PoisonDamageMultiplier));

            if (isMaster)
            {
                baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MasterExperienceRate));
            }

            if (!this.UseClassicPvp)
            {
                baseAttributeValues.Add(this.CreateConstValueAttribute(0.01f, Stats.ShieldRecoveryMultiplier));
            }
        }
    }
}
