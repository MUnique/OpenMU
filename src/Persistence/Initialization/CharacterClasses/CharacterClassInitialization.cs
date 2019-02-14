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
            // var dragonKnight = this.CreateDragonKnight();
            var bladeMaster = this.CreateBladeMaster(/*dragonKnight*/);
            var bladeKnight = this.CreateBladeKnight(bladeMaster);
            this.CreateDarkKnight(CharacterClassNumber.DarkKnight, "Dark Knight", 5, false, bladeKnight, true);

            // var soulWizard = this.CreateSoulWizard();
            var grandMaster = this.CreateGrandMaster(/*soulWizard*/);
            var soulMaster = this.CreateSoulMaster(grandMaster);
            this.CreateDarkWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", 5, false, soulMaster, true);

            // var nobleElf = this.CreateNobleElf();
            var highElf = this.CreateHighElf(/*nobleElf*/);
            var museElf = this.CreateMuseElf(highElf);
            this.CreateFairyElf(CharacterClassNumber.FairyElf, "Fairy Elf", 5, false, museElf, true);

            // dimensionSummoner = this.CreateDimensionSummoner();
            var dimensionMaster = this.CreateDimensionMaster(/*dimensionSummoner*/);
            var bloodySummoner = this.CreateBloodySummoner(dimensionMaster);
            this.CreateSummoner(CharacterClassNumber.Summoner, "Summoner", 5, false, bloodySummoner, true);

            // var magicKnight = this.CreateMagicKnight();
            var duelMaster = this.CreateDuelMaster(/*magicKnight*/);
            this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, duelMaster, true);

            // var empireRoad = this.CreateEmpireRoad();
            var lordEmperor = this.CreateLordEmperor(/*empireRoad*/);
            this.CreateDarkLord(CharacterClassNumber.DarkLord, "Dark Lord", 7, false, lordEmperor, true);

            // var fistBlazer = this.CreateFistBlazer();
            var fistMaster = this.CreateFistMaster(/*fistBlazer*/);
            this.CreateRageFighter(CharacterClassNumber.RageFighter, "Rage Fighter", false, fistMaster, true);

            // var shiningLancer = this.CreateShiningLancer();
            // var mirageLancer = this.CreateMirageLancer(shiningLancer);
            // this.CreateGrowLancer(CharacterClassNumber.GrowLancer, "Grow Lancer", 7, false, mirageLancer, true);
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
