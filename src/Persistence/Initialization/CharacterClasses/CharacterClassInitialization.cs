// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;

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
        /// Creates the character classes.
        /// </summary>
        public override void Initialize()
        {
            var bladeMaster = this.CreateBladeMaster();
            var bladeKnight = this.CreateBladeKnight(bladeMaster);
            this.CreateDarkKnight(CharacterClassNumber.DarkKnight, "Dark Knight", 5, false, bladeKnight, true);

            var grandMaster = this.CreateGrandMaster();
            var soulMaster = this.CreateSoulMaster(grandMaster);
            this.CreateDarkWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", 5, false, soulMaster, true);

            var highElf = this.CreateHighElf();
            var museElf = this.CreateMuseElf(highElf);
            this.CreateFairyElf(CharacterClassNumber.FairyElf, "Fairy Elf", 5, false, museElf, true);

            var dimensionMaster = this.CreateDimensionMaster();
            var bloodySummoner = this.CreateBloodySummoner(dimensionMaster);
            this.CreateSummoner(CharacterClassNumber.Summoner, "Summoner", 5, false, bloodySummoner, true);

            var duelMaster = this.CreateDuelMaster();
            this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, duelMaster, true);

            var lordEmperor = this.CreateLordEmperor();
            this.CreateDarkLord(CharacterClassNumber.DarkLord, "Dark Lord", 7, false, lordEmperor, true);

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
    }
}
